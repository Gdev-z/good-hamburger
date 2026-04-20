using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public MenuItemType Type { get; set; }
}
