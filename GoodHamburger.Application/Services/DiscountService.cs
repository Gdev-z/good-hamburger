using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Services;

/// <summary>
/// Isolates all discount calculation logic.
/// Rules:
///   Sandwich + SideDish + Drink  → 20% off
///   Sandwich + Drink             → 15% off
///   Sandwich + SideDish          → 10% off
///   Sandwich only                → no discount
/// </summary>
public static class DiscountService
{
    public static (decimal subtotal, decimal discount, decimal total) Calculate(IEnumerable<MenuItem> items)
    {
        var list = items.ToList();
        bool hasSandwich = list.Any(i => i.Type == MenuItemType.Sandwich);
        bool hasSideDish  = list.Any(i => i.Type == MenuItemType.SideDish);
        bool hasDrink     = list.Any(i => i.Type == MenuItemType.Drink);

        decimal subtotal = list.Sum(i => i.Price);

        decimal discountRate = (hasSandwich, hasSideDish, hasDrink) switch
        {
            (true, true, true)   => 0.20m,
            (true, false, true)  => 0.15m,
            (true, true, false)  => 0.10m,
            _                    => 0.00m
        };

        decimal discount = Math.Round(subtotal * discountRate, 2);
        decimal total    = subtotal - discount;

        return (subtotal, discount, total);
    }
}
