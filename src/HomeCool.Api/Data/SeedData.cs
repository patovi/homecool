using HomeCool.Api.Entities;

namespace HomeCool.Api.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var admin = new User
        {
            Name = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin",
            IsActive = true
        };
        var alice = new User
        {
            Name = "Alice",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Role = "User",
            IsActive = true
        };
        var bob = new User
        {
            Name = "Bob",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Role = "User",
            IsActive = true
        };

        db.Users.AddRange(admin, alice, bob);

        var cola = new Product { Name = "Cola", Price = 1.50m, Stock = 24, IsActive = true };
        var water = new Product { Name = "Wasser", Price = 0.80m, Stock = 12, IsActive = true };
        var beer = new Product { Name = "Bier", Price = 2.00m, Stock = 20, IsActive = true };
        var juice = new Product { Name = "Apfelsaft", Price = 1.20m, Stock = 8, IsActive = true };

        db.Products.AddRange(cola, water, beer, juice);
        await db.SaveChangesAsync();

        // Demo purchase
        var purchase = new Purchase
        {
            CreatedByUserId = admin.Id,
            Date = DateTime.UtcNow.AddDays(-7),
            TotalCost = 48.80m,
            Note = "Wocheneinkauf",
            Items =
            [
                new PurchaseItem { ProductId = cola.Id, Quantity = 24, UnitCost = 0.85m },
                new PurchaseItem { ProductId = water.Id, Quantity = 12, UnitCost = 0.50m },
                new PurchaseItem { ProductId = beer.Id, Quantity = 20, UnitCost = 1.20m },
                new PurchaseItem { ProductId = juice.Id, Quantity = 8, UnitCost = 0.90m },
            ]
        };
        db.Purchases.Add(purchase);

        // Demo consumptions
        db.Consumptions.AddRange(
            new Consumption { UserId = alice.Id, ProductId = cola.Id, Quantity = 3, PriceAtTime = 1.50m, ConsumedAt = DateTime.UtcNow.AddDays(-5) },
            new Consumption { UserId = alice.Id, ProductId = beer.Id, Quantity = 2, PriceAtTime = 2.00m, ConsumedAt = DateTime.UtcNow.AddDays(-3) },
            new Consumption { UserId = bob.Id, ProductId = water.Id, Quantity = 4, PriceAtTime = 0.80m, ConsumedAt = DateTime.UtcNow.AddDays(-4) },
            new Consumption { UserId = bob.Id, ProductId = cola.Id, Quantity = 2, PriceAtTime = 1.50m, ConsumedAt = DateTime.UtcNow.AddDays(-2) }
        );

        // Demo payment
        db.Payments.Add(new Payment { UserId = alice.Id, Amount = 10.00m, PaidAt = DateTime.UtcNow.AddDays(-1), Note = "Überweisung" });

        await db.SaveChangesAsync();
    }
}
