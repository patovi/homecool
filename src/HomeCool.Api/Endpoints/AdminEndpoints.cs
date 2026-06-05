using HomeCool.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin").RequireAuthorization("AdminOnly");

        group.MapDelete("/demo-data", async (AppDbContext db) =>
        {
            // Order matters due to FK constraints
            await db.Consumptions.ExecuteDeleteAsync();
            await db.PurchaseItems.ExecuteDeleteAsync();
            await db.Purchases.ExecuteDeleteAsync();
            await db.Payments.ExecuteDeleteAsync();
            await db.MonthlySettlements.ExecuteDeleteAsync();

            // Reset product stock to 0, keep the products themselves
            await db.Products.ExecuteUpdateAsync(p =>
                p.SetProperty(x => x.Stock, 0));

            return Results.Ok(new { message = "Demodaten wurden gelöscht. Produkte und Benutzer bleiben erhalten." });
        });
    }
}
