using FluentAssertions;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Tests.Services;

public class DiscountServiceTests
{
    // ── helpers ──────────────────────────────────────────────────────────────
    private static MenuItem Sandwich(decimal price = 5.00m) =>
        new() { Id = 1, Name = "X Burger", Price = price, Type = MenuItemType.Sandwich };

    private static MenuItem SideDish(decimal price = 2.00m) =>
        new() { Id = 4, Name = "Batata Frita", Price = price, Type = MenuItemType.SideDish };

    private static MenuItem Drink(decimal price = 2.50m) =>
        new() { Id = 5, Name = "Refrigerante", Price = price, Type = MenuItemType.Drink };

    // ── Discount rules ────────────────────────────────────────────────────────

    [Fact]
    public void Calculate_SandwichSideDishDrink_AppliesTwentyPercentDiscount()
    {
        // Arrange: X Burger (5.00) + Batata (2.00) + Refrigerante (2.50) = 9.50
        var items = new[] { Sandwich(), SideDish(), Drink() };

        // Act
        var (subtotal, discount, total) = DiscountService.Calculate(items);

        // Assert
        subtotal.Should().Be(9.50m);
        discount.Should().Be(1.90m);    // 9.50 * 0.20
        total.Should().Be(7.60m);
    }

    [Fact]
    public void Calculate_SandwichAndDrink_AppliesFifteenPercentDiscount()
    {
        // Arrange: X Bacon (7.00) + Refrigerante (2.50) = 9.50
        var items = new[]
        {
            new MenuItem { Id = 3, Name = "X Bacon", Price = 7.00m, Type = MenuItemType.Sandwich },
            Drink()
        };

        // Act
        var (subtotal, discount, total) = DiscountService.Calculate(items);

        // Assert
        subtotal.Should().Be(9.50m);
        // Math.Round(9.50 * 0.15, 2) = 1.42 (banker's rounding on .5)
        discount.Should().Be(Math.Round(9.50m * 0.15m, 2));
        total.Should().Be(subtotal - discount);
    }

    [Fact]
    public void Calculate_SandwichAndSideDish_AppliesTenPercentDiscount()
    {
        // Arrange: X Egg (4.50) + Batata (2.00) = 6.50
        var items = new[]
        {
            new MenuItem { Id = 2, Name = "X Egg", Price = 4.50m, Type = MenuItemType.Sandwich },
            SideDish()
        };

        // Act
        var (subtotal, discount, total) = DiscountService.Calculate(items);

        // Assert
        subtotal.Should().Be(6.50m);
        discount.Should().Be(0.65m);    // 6.50 * 0.10
        total.Should().Be(5.85m);
    }

    [Fact]
    public void Calculate_SandwichOnly_AppliesNoDiscount()
    {
        var items = new[] { Sandwich(5.00m) };

        var (subtotal, discount, total) = DiscountService.Calculate(items);

        subtotal.Should().Be(5.00m);
        discount.Should().Be(0.00m);
        total.Should().Be(5.00m);
    }

    [Fact]
    public void Calculate_SideDishAndDrinkWithoutSandwich_AppliesNoDiscount()
    {
        // No sandwich → no discount regardless of combination
        var items = new[] { SideDish(), Drink() };

        var (subtotal, discount, total) = DiscountService.Calculate(items);

        subtotal.Should().Be(4.50m);
        discount.Should().Be(0.00m);
        total.Should().Be(4.50m);
    }
}
