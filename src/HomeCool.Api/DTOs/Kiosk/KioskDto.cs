namespace HomeCool.Api.DTOs.Kiosk;

public record KioskBookRequest(int UserId, int ProductId, int Quantity, string Pin);

public record KioskBookResponse(string ProductName, string UserName, int Quantity, decimal PriceAtTime, decimal Total);
