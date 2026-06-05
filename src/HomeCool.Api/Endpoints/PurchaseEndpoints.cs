using System.Security.Claims;
using HomeCool.Api.DTOs.Purchases;
using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class PurchaseEndpoints
{
    public static void MapPurchaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchases").WithTags("Purchases").RequireAuthorization();

        group.MapGet("/", async (PurchaseService service) =>
            Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, PurchaseService service) =>
        {
            var purchase = await service.GetByIdAsync(id);
            return purchase is null ? Results.NotFound() : Results.Ok(purchase);
        });

        group.MapPost("/", async (CreatePurchaseRequest request, ClaimsPrincipal user, PurchaseService service) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (purchase, error) = await service.CreateAsync(userId, request);
            return error is not null
                ? Results.BadRequest(new { error })
                : Results.Created($"/api/purchases/{purchase!.Id}", purchase);
        }).RequireAuthorization("AdminOnly");
    }
}
