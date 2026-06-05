using System.Security.Claims;
using HomeCool.Api.DTOs.Consumptions;
using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class ConsumptionEndpoints
{
    public static void MapConsumptionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/consumptions").WithTags("Consumptions").RequireAuthorization();

        group.MapGet("/", async (ConsumptionService service, int? userId) =>
            Results.Ok(await service.GetAllAsync(userId)));

        group.MapPost("/", async (CreateConsumptionRequest request, ClaimsPrincipal user, ConsumptionService service) =>
        {
            var isAdmin = user.IsInRole("Admin");
            var (consumption, error) = await service.CreateAsync(request, isAdmin);
            return error is not null
                ? Results.BadRequest(new { error })
                : Results.Created($"/api/consumptions/{consumption!.Id}", consumption);
        });

        group.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, ConsumptionService service) =>
        {
            var isAdmin = user.IsInRole("Admin");
            var (success, error) = await service.DeleteAsync(id, isAdmin);
            if (error is not null) return Results.BadRequest(new { error });
            return success ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization("AdminOnly");
    }
}
