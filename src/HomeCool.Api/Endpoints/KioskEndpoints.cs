using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Kiosk;
using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Endpoints;

public static class KioskEndpoints
{
    public static void MapKioskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kiosk").WithTags("Kiosk");

        // Returns active users and products for the kiosk UI (no auth needed)
        group.MapGet("/data", async (AppDbContext db) =>
        {
            var users = await db.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Name)
                .Select(u => new { u.Id, u.Name })
                .ToListAsync();

            var products = await db.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new { p.Id, p.Name, p.Price, p.Stock })
                .ToListAsync();

            return Results.Ok(new { users, products });
        }).AllowAnonymous();

        // Book a consumption – PIN replaces JWT auth
        group.MapPost("/book", async (KioskBookRequest request, AppDbContext db) =>
        {
            if (request.Quantity <= 0)
                return Results.BadRequest(new { error = "Menge muss größer als 0 sein." });

            var user = await db.Users.FindAsync(request.UserId);
            if (user is null || !user.IsActive)
                return Results.BadRequest(new { error = "Benutzer nicht gefunden." });

            if (!BCrypt.Net.BCrypt.Verify(request.Pin, user.PasswordHash))
                return Results.BadRequest(new { error = "Falscher PIN." });

            var product = await db.Products.FindAsync(request.ProductId);
            if (product is null || !product.IsActive)
                return Results.BadRequest(new { error = "Produkt nicht verfügbar." });

            if (product.Stock < request.Quantity)
                return Results.BadRequest(new { error = $"Nicht genug Bestand. Verfügbar: {product.Stock}" });

            product.Stock -= request.Quantity;

            var consumption = new Consumption
            {
                UserId = user.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                PriceAtTime = product.Price,
                ConsumedAt = DateTime.UtcNow
            };

            db.Consumptions.Add(consumption);
            await db.SaveChangesAsync();

            return Results.Ok(new KioskBookResponse(
                product.Name,
                user.Name,
                request.Quantity,
                product.Price,
                request.Quantity * product.Price));
        }).AllowAnonymous();
    }
}
