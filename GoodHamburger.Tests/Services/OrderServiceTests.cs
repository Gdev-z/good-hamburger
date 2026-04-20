using FluentAssertions;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;
using Moq;

namespace GoodHamburger.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IMenuRepository>  _menuRepoMock  = new();

    private OrderService CreateService() =>
        new(_orderRepoMock.Object, _menuRepoMock.Object);

    private static List<MenuItem> DefaultMenu() =>
    [
        new() { Id = 1, Name = "X Burger",     Price = 5.00m, Type = MenuItemType.Sandwich },
        new() { Id = 2, Name = "X Egg",        Price = 4.50m, Type = MenuItemType.Sandwich },
        new() { Id = 3, Name = "X Bacon",      Price = 7.00m, Type = MenuItemType.Sandwich },
        new() { Id = 4, Name = "Batata Frita", Price = 2.00m, Type = MenuItemType.SideDish },
        new() { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Drink    }
    ];

    // ── Duplicate category validation ─────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_DuplicateSandwich_ThrowsDomainException()
    {
        // Arrange: two sandwiches (ids 1 and 2)
        var menu = DefaultMenu().Where(m => m.Id == 1 || m.Id == 2).ToList();
        _menuRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                     .ReturnsAsync(menu);

        var svc = CreateService();
        var dto = new CreateOrderDto { MenuItemIds = [1, 2] };

        // Act & Assert
        await svc.Invoking(s => s.CreateOrderAsync(dto))
                 .Should().ThrowAsync<DomainException>()
                 .WithMessage("*sanduíche*");
    }

    [Fact]
    public async Task CreateOrder_DuplicateSideDish_ThrowsDomainException()
    {
        // Arrange: one sandwich + two side-dishes (we simulate by duplicating id 4)
        var menu = new List<MenuItem>
        {
            new() { Id = 1, Name = "X Burger",     Price = 5.00m, Type = MenuItemType.Sandwich },
            new() { Id = 4, Name = "Batata Frita", Price = 2.00m, Type = MenuItemType.SideDish },
            new() { Id = 4, Name = "Batata Frita", Price = 2.00m, Type = MenuItemType.SideDish }
        };
        _menuRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                     .ReturnsAsync(menu);

        var svc = CreateService();
        var dto = new CreateOrderDto { MenuItemIds = [1, 4, 4] };

        await svc.Invoking(s => s.CreateOrderAsync(dto))
                 .Should().ThrowAsync<DomainException>()
                 .WithMessage("*batata*");
    }

    [Fact]
    public async Task CreateOrder_DuplicateDrink_ThrowsDomainException()
    {
        var menu = new List<MenuItem>
        {
            new() { Id = 1, Name = "X Burger",     Price = 5.00m, Type = MenuItemType.Sandwich },
            new() { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Drink    },
            new() { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Drink    }
        };
        _menuRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                     .ReturnsAsync(menu);

        var svc = CreateService();
        var dto = new CreateOrderDto { MenuItemIds = [1, 5, 5] };

        await svc.Invoking(s => s.CreateOrderAsync(dto))
                 .Should().ThrowAsync<DomainException>()
                 .WithMessage("*refrigerante*");
    }

    // ── Discount calculation via service ──────────────────────────────────────

    [Fact]
    public async Task CreateOrder_FullCombo_Applies20PercentDiscount()
    {
        // X Burger(5) + Batata(2) + Refrigerante(2.5) = 9.5 → 20% off = 7.60
        var menu = DefaultMenu().Where(m => new[] { 1, 4, 5 }.Contains(m.Id)).ToList();
        _menuRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                     .ReturnsAsync(menu);

        var expectedOrder = new Order
        {
            Id       = 1,
            Items    = menu.Select(m => new OrderItem { MenuItemId = m.Id, MenuItem = m }).ToList(),
            Subtotal = 9.50m,
            Discount = 1.90m,
            Total    = 7.60m
        };
        _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                      .ReturnsAsync(expectedOrder);

        var svc = CreateService();
        var result = await svc.CreateOrderAsync(new CreateOrderDto { MenuItemIds = [1, 4, 5] });

        result.Subtotal.Should().Be(9.50m);
        result.Discount.Should().Be(1.90m);
        result.Total.Should().Be(7.60m);
    }

    [Fact]
    public async Task CreateOrder_InvalidMenuItemId_ThrowsDomainException()
    {
        _menuRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                     .ReturnsAsync(new List<MenuItem>());   // nothing found

        var svc = CreateService();
        var dto = new CreateOrderDto { MenuItemIds = [99] };

        await svc.Invoking(s => s.CreateOrderAsync(dto))
                 .Should().ThrowAsync<DomainException>()
                 .WithMessage("*inválido*");
    }

    [Fact]
    public async Task GetOrderById_NonExistentId_ThrowsNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync((Order?)null);

        var svc = CreateService();

        await svc.Invoking(s => s.GetOrderByIdAsync(999))
                 .Should().ThrowAsync<NotFoundException>();
    }
}
