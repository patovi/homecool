using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Purchases;
using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Services;

public class PurchaseService(AppDbContext db)
{
    public async Task<List<PurchaseDto>> GetAllAsync() =>
        await db.Purchases
            .Include(p => p.CreatedByUser)
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .OrderByDescending(p => p.Date)
            .Select(p => MapToDto(p))
            .ToListAsync();

    public async Task<PurchaseDto?> GetByIdAsync(int id)
    {
        var purchase = await db.Purchases
            .Include(p => p.CreatedByUser)
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

        return purchase is null ? null : MapToDto(purchase);
    }

    public async Task<(PurchaseDto? Purchase, string? Error)> CreateAsync(int adminUserId, CreatePurchaseRequest request)
    {
        if (request.Items.Count == 0)
            return (null, "Mindestens eine Position erforderlich.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await db.Products.Where(p => productIds.Contains(p.Id) && p.IsActive).ToListAsync();

        if (products.Count != productIds.Count)
            return (null, "Ein oder mehrere Produkte sind nicht vorhanden oder inaktiv.");

        var purchase = new Purchase
        {
            CreatedByUserId = adminUserId,
            Date = DateTime.UtcNow,
            Note = request.Note
        };

        decimal total = 0;
        foreach (var itemRequest in request.Items)
        {
            var product = products.First(p => p.Id == itemRequest.ProductId);
            product.Stock += itemRequest.Quantity;
            total += itemRequest.Quantity * itemRequest.UnitCost;

            purchase.Items.Add(new PurchaseItem
            {
                ProductId = itemRequest.ProductId,
                Quantity = itemRequest.Quantity,
                UnitCost = itemRequest.UnitCost
            });
        }

        purchase.TotalCost = total;
        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        var result = await GetByIdAsync(purchase.Id);
        return (result, null);
    }

    private static PurchaseDto MapToDto(Purchase p) => new(
        p.Id,
        p.CreatedByUserId,
        p.CreatedByUser.Name,
        p.Date,
        p.TotalCost,
        p.Note,
        p.Items.Select(i => new PurchaseItemDto(i.ProductId, i.Product.Name, i.Quantity, i.UnitCost)).ToList());
}
