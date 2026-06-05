using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Consumptions;
using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Services;

public class ConsumptionService(AppDbContext db)
{
    public async Task<List<ConsumptionDto>> GetAllAsync(int? userId = null)
    {
        var query = db.Consumptions
            .Include(c => c.User)
            .Include(c => c.Product)
            .AsQueryable();

        if (userId.HasValue) query = query.Where(c => c.UserId == userId.Value);

        return await query
            .OrderByDescending(c => c.ConsumedAt)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<(ConsumptionDto? Consumption, string? Error)> CreateAsync(CreateConsumptionRequest request, bool isAdmin)
    {
        var user = await db.Users.FindAsync(request.UserId);
        if (user is null || !user.IsActive)
            return (null, "Benutzer nicht gefunden oder inaktiv.");

        var product = await db.Products.FindAsync(request.ProductId);
        if (product is null || !product.IsActive)
            return (null, "Produkt nicht gefunden oder inaktiv.");

        if (request.Quantity <= 0)
            return (null, "Menge muss größer als 0 sein.");

        if (product.Stock < request.Quantity)
        {
            if (!isAdmin || !request.AllowNegativeStock)
                return (null, $"Nicht genug Bestand. Verfügbar: {product.Stock}");
        }

        product.Stock -= request.Quantity;

        var consumption = new Consumption
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            PriceAtTime = product.Price,
            ConsumedAt = DateTime.UtcNow
        };

        db.Consumptions.Add(consumption);
        await db.SaveChangesAsync();

        await db.Entry(consumption).Reference(c => c.User).LoadAsync();
        await db.Entry(consumption).Reference(c => c.Product).LoadAsync();

        return (MapToDto(consumption), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id, bool isAdmin)
    {
        var consumption = await db.Consumptions.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == id);
        if (consumption is null) return (false, "Buchung nicht gefunden.");

        // Restore stock
        consumption.Product.Stock += consumption.Quantity;
        db.Consumptions.Remove(consumption);
        await db.SaveChangesAsync();
        return (true, null);
    }

    private static ConsumptionDto MapToDto(Consumption c) => new(
        c.Id,
        c.UserId,
        c.User.Name,
        c.ProductId,
        c.Product.Name,
        c.Quantity,
        c.PriceAtTime,
        c.Quantity * c.PriceAtTime,
        c.ConsumedAt);
}
