namespace GoodHamburger.Application.DTOs;

public class CreateOrderDto
{
    public List<int> MenuItemIds { get; set; } = new();
}
