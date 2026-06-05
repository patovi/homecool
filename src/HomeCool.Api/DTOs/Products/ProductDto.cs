namespace HomeCool.Api.DTOs.Products;

public record ProductDto(int Id, string Name, decimal Price, int Stock, bool IsActive, DateTime CreatedAt);

public record CreateProductRequest(string Name, decimal Price, int InitialStock = 0);

public record UpdateProductRequest(string? Name, decimal? Price, bool? IsActive);

public record AdjustStockRequest(int Adjustment, string? Note);
