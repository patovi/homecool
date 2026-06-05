namespace HomeCool.Api.DTOs.Consumptions;

public record ConsumptionDto(
    int Id,
    int UserId,
    string UserName,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal PriceAtTime,
    decimal Total,
    DateTime ConsumedAt);

public record CreateConsumptionRequest(int UserId, int ProductId, int Quantity, bool AllowNegativeStock = false);
