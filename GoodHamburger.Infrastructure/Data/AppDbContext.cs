using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MenuItem>  MenuItems  { get; set; }
    public DbSet<Order>     Orders     { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed the menu
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "X Burger",     Price = 5.00m, Type = MenuItemType.Sandwich },
            new MenuItem { Id = 2, Name = "X Egg",        Price = 4.50m, Type = MenuItemType.Sandwich },
            new MenuItem { Id = 3, Name = "X Bacon",      Price = 7.00m, Type = MenuItemType.Sandwich },
            new MenuItem { Id = 4, Name = "Batata Frita", Price = 2.00m, Type = MenuItemType.SideDish },
            new MenuItem { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Drink    }
        );

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId);
    }
}
