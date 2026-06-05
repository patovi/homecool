using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Products;
using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Services;

public class ProductService(AppDbContext db)
{
    public async Task<List<ProductDto>> GetAllAsync(bool onlyActive = false)
    {
        var query = db.Products.AsQueryable();
        if (onlyActive) query = query.Where(p => p.IsActive);
        return await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock, p.IsActive, p.CreatedAt))
            .ToListAsync();
    }

    public async Task<ProductDto?> GetByIdAsync(int id) =>
        await db.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock, p.IsActive, p.CreatedAt))
            .FirstOrDefaultAsync();

    public async Task<(ProductDto? Product, string? Error)> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.InitialStock
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return (new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.IsActive, product.CreatedAt), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return (false, "Produkt nicht gefunden.");

        bool hasConsumptions = await db.Consumptions.AnyAsync(c => c.ProductId == id);
        bool hasPurchaseItems = await db.PurchaseItems.AnyAsync(p => p.ProductId == id);

        if (hasConsumptions || hasPurchaseItems)
            return (false, "Produkt kann nicht gelöscht werden, da bereits Buchungen oder Einkäufe vorhanden sind. Bitte deaktiviere es stattdessen.");

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(ProductDto? Product, string? Error)> AdjustStockAsync(int id, int adjustment)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return (null, "Produkt nicht gefunden.");

        var newStock = product.Stock + adjustment;
        if (newStock < 0)
            return (null, $"Bestand kann nicht negativ werden. Aktuell: {product.Stock}");

        product.Stock = newStock;
        await db.SaveChangesAsync();
        return (new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.IsActive, product.CreatedAt), null);
    }

    public async Task<(ProductDto? Product, string? Error)> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return (null, "Produkt nicht gefunden.");

        if (request.Name is not null) product.Name = request.Name;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync();
        return (new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.IsActive, product.CreatedAt), null);
    }
}
