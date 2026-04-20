using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
        => await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.MenuItem)
            .ToListAsync();

    public async Task<Order?> GetByIdAsync(int id)
        => await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        return (await GetByIdAsync(order.Id))!;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        // Remove existing items then replace
        var existingItems = _context.OrderItems.Where(oi => oi.OrderId == order.Id);
        _context.OrderItems.RemoveRange(existingItems);

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(order.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is not null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
