using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto> UpdateOrderAsync(int id, CreateOrderDto dto);
    Task DeleteOrderAsync(int id);
}
