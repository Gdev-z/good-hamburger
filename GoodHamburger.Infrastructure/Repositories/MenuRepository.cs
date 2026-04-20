using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;

    public MenuRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync()
        => await _context.MenuItems.ToListAsync();

    public async Task<MenuItem?> GetByIdAsync(int id)
        => await _context.MenuItems.FindAsync(id);

    public async Task<IEnumerable<MenuItem>> GetByIdsAsync(IEnumerable<int> ids)
        => await _context.MenuItems.Where(m => ids.Contains(m.Id)).ToListAsync();
}
