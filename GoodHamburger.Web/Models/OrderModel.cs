namespace GoodHamburger.Web.Models;

public class OrderItemModel
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class OrderModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemModel> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}

public class CreateOrderModel
{
    public List<int> MenuItemIds { get; set; } = new();
}
