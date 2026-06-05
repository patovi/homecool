using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports").RequireAuthorization();

        group.MapGet("/stock", async (ReportService service) =>
            Results.Ok(await service.GetStockAsync()));

        group.MapGet("/user-balance", async (ReportService service) =>
            Results.Ok(await service.GetUserBalancesAsync()));

        group.MapGet("/monthly-summary", async (ReportService service, int? year, int? month) =>
        {
            var now = DateTime.UtcNow;
            var result = await service.GetMonthlySummaryAsync(
                year ?? now.Year,
                month ?? now.Month);
            return Results.Ok(result);
        });

        group.MapGet("/export/monthly-summary", async (ReportService service, int? year, int? month) =>
        {
            var now = DateTime.UtcNow;
            var csv = await service.GetMonthlySummaryCsvAsync(
                year ?? now.Year,
                month ?? now.Month);
            var fileName = $"abrechnung-{year ?? now.Year}-{(month ?? now.Month):D2}.csv";
            return Results.File(
                System.Text.Encoding.UTF8.GetBytes(csv),
                "text/csv",
                fileName);
        });
    }
}
