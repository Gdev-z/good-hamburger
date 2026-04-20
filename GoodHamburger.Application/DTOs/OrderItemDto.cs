namespace GoodHamburger.Application.DTOs;

public class OrderItemDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
