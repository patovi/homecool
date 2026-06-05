using System.Globalization;
using System.Text;
using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Services;

public class ReportService(AppDbContext db)
{
    public async Task<List<StockItem>> GetStockAsync() =>
        await db.Products
            .OrderBy(p => p.Name)
            .Select(p => new StockItem(p.Id, p.Name, p.Stock, p.Price, p.IsActive))
            .ToListAsync();

    public async Task<List<UserBalanceDto>> GetUserBalancesAsync()
    {
        var users = await db.Users.Where(u => u.IsActive).ToListAsync();

        var consumptionTotals = await db.Consumptions
            .GroupBy(c => c.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(c => c.Quantity * c.PriceAtTime) })
            .ToListAsync();

        var paymentTotals = await db.Payments
            .GroupBy(p => p.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync();

        return users.Select(u =>
        {
            var consumed = consumptionTotals.FirstOrDefault(c => c.UserId == u.Id)?.Total ?? 0;
            var paid = paymentTotals.FirstOrDefault(p => p.UserId == u.Id)?.Total ?? 0;
            return new UserBalanceDto(u.Id, u.Name, consumed, paid, paid - consumed);
        }).OrderBy(u => u.UserName).ToList();
    }

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(int year, int month)
    {
        var users = await db.Users.ToListAsync();

        var consumptions = await db.Consumptions
            .Where(c => c.ConsumedAt.Year == year && c.ConsumedAt.Month == month)
            .GroupBy(c => c.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(c => c.Quantity * c.PriceAtTime) })
            .ToListAsync();

        var payments = await db.Payments
            .Where(p => p.PaidAt.Year == year && p.PaidAt.Month == month)
            .GroupBy(p => p.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync();

        var rows = users
            .Where(u => consumptions.Any(c => c.UserId == u.Id) || payments.Any(p => p.UserId == u.Id))
            .Select(u =>
            {
                var consumed = consumptions.FirstOrDefault(c => c.UserId == u.Id)?.Total ?? 0;
                var paid = payments.FirstOrDefault(p => p.UserId == u.Id)?.Total ?? 0;
                return new MonthlySummaryUserRow(u.Id, u.Name, consumed, paid, paid - consumed);
            })
            .OrderBy(r => r.UserName)
            .ToList();

        return new MonthlySummaryDto(year, month, rows);
    }

    public async Task<string> GetMonthlySummaryCsvAsync(int year, int month)
    {
        var summary = await GetMonthlySummaryAsync(year, month);
        var sb = new StringBuilder();
        sb.AppendLine("Benutzer;Konsum (€);Gezahlt (€);Saldo (€)");
        foreach (var row in summary.Rows)
        {
            sb.AppendLine(string.Join(";",
                row.UserName,
                row.TotalConsumed.ToString("F2", CultureInfo.InvariantCulture),
                row.TotalPaid.ToString("F2", CultureInfo.InvariantCulture),
                row.Balance.ToString("F2", CultureInfo.InvariantCulture)));
        }
        return sb.ToString();
    }
}
