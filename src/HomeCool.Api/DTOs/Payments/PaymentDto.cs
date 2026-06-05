namespace HomeCool.Api.DTOs.Payments;

public record PaymentDto(int Id, int UserId, string UserName, decimal Amount, DateTime PaidAt, string? Note);

public record CreatePaymentRequest(int UserId, decimal Amount, string? Note);
