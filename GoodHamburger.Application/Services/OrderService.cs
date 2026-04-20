using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuRepository  _menuRepository;

    public OrderService(IOrderRepository orderRepository, IMenuRepository menuRepository)
    {
        _orderRepository = orderRepository;
        _menuRepository  = menuRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Pedido com ID {id} não encontrado.");
        return MapToDto(order);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        if (dto.MenuItemIds == null || dto.MenuItemIds.Count == 0)
            throw new DomainException("O pedido deve conter ao menos um item.");

        var menuItems = (await _menuRepository.GetByIdsAsync(dto.MenuItemIds)).ToList();

        ValidateItemIds(dto.MenuItemIds, menuItems);
        ValidateDuplicateCategories(menuItems);

        var (subtotal, discount, total) = DiscountService.Calculate(menuItems);

        var order = new Order
        {
            CreatedAt = DateTime.UtcNow,
            Items = menuItems.Select(m => new OrderItem { MenuItemId = m.Id }).ToList(),
            Subtotal  = subtotal,
            Discount  = discount,
            Total     = total
        };

        var created = await _orderRepository.AddAsync(order);
        return MapToDto(created);
    }

    public async Task<OrderDto> UpdateOrderAsync(int id, CreateOrderDto dto)
    {
        var existing = await _orderRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Pedido com ID {id} não encontrado.");

        if (dto.MenuItemIds == null || dto.MenuItemIds.Count == 0)
            throw new DomainException("O pedido deve conter ao menos um item.");

        var menuItems = (await _menuRepository.GetByIdsAsync(dto.MenuItemIds)).ToList();

        ValidateItemIds(dto.MenuItemIds, menuItems);
        ValidateDuplicateCategories(menuItems);

        var (subtotal, discount, total) = DiscountService.Calculate(menuItems);

        existing.Items     = menuItems.Select(m => new OrderItem { MenuItemId = m.Id, OrderId = id }).ToList();
        existing.Subtotal  = subtotal;
        existing.Discount  = discount;
        existing.Total     = total;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _orderRepository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    public async Task DeleteOrderAsync(int id)
    {
        _ = await _orderRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Pedido com ID {id} não encontrado.");
        await _orderRepository.DeleteAsync(id);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static void ValidateItemIds(List<int> requestedIds, List<MenuItem> found)
    {
        var foundIds    = found.Select(m => m.Id).ToHashSet();
        var invalidIds  = requestedIds.Where(id => !foundIds.Contains(id)).ToList();
        if (invalidIds.Any())
            throw new DomainException($"Item(ns) inválido(s) no cardápio: {string.Join(", ", invalidIds)}.");
    }

    private static void ValidateDuplicateCategories(List<MenuItem> items)
    {
        var sandwiches = items.Where(i => i.Type == MenuItemType.Sandwich).ToList();
        var sideDishes = items.Where(i => i.Type == MenuItemType.SideDish).ToList();
        var drinks     = items.Where(i => i.Type == MenuItemType.Drink).ToList();

        var errors = new List<string>();
        if (sandwiches.Count > 1) errors.Add("apenas um sanduíche é permitido por pedido");
        if (sideDishes.Count  > 1) errors.Add("apenas uma batata frita é permitida por pedido");
        if (drinks.Count      > 1) errors.Add("apenas um refrigerante é permitido por pedido");

        if (errors.Any())
            throw new DomainException($"Pedido inválido: {string.Join("; ", errors)}.");
    }

    private static OrderDto MapToDto(Order order) => new()
    {
        Id        = order.Id,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt,
        Items     = order.Items.Select(oi => new OrderItemDto
        {
            MenuItemId   = oi.MenuItemId,
            MenuItemName = oi.MenuItem?.Name ?? string.Empty,
            Price        = oi.MenuItem?.Price ?? 0
        }).ToList(),
        Subtotal = order.Subtotal,
        Discount = order.Discount,
        Total    = order.Total
    };
}
