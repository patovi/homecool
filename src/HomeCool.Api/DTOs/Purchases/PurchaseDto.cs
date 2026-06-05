namespace HomeCool.Api.DTOs.Purchases;

public record PurchaseItemDto(int ProductId, string ProductName, int Quantity, decimal UnitCost);

public record PurchaseDto(
    int Id,
    int CreatedByUserId,
    string CreatedByUserName,
    DateTime Date,
    decimal TotalCost,
    string? Note,
    List<PurchaseItemDto> Items);

public record CreatePurchaseItemRequest(int ProductId, int Quantity, decimal UnitCost);

public record CreatePurchaseRequest(List<CreatePurchaseItemRequest> Items, string? Note);
