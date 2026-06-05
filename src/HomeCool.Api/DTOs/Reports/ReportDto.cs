namespace HomeCool.Api.DTOs.Reports;

public record StockItem(int ProductId, string ProductName, int Stock, decimal Price, bool IsActive);

public record UserBalanceDto(int UserId, string UserName, decimal TotalConsumed, decimal TotalPaid, decimal Balance);

public record MonthlySummaryUserRow(
    int UserId,
    string UserName,
    decimal TotalConsumed,
    decimal TotalPaid,
    decimal Balance);

public record MonthlySummaryDto(int Year, int Month, List<MonthlySummaryUserRow> Rows);
