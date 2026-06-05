using HomeCool.Api.DTOs.Products;
using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products").RequireAuthorization();

        group.MapGet("/", async (ProductService service, bool onlyActive = false) =>
            Results.Ok(await service.GetAllAsync(onlyActive)));

        group.MapGet("/{id:int}", async (int id, ProductService service) =>
        {
            var product = await service.GetByIdAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("/", async (CreateProductRequest request, ProductService service) =>
        {
            var (product, error) = await service.CreateAsync(request);
            return error is not null
                ? Results.BadRequest(new { error })
                : Results.Created($"/api/products/{product!.Id}", product);
        }).RequireAuthorization("AdminOnly");

        group.MapPut("/{id:int}", async (int id, UpdateProductRequest request, ProductService service) =>
        {
            var (product, error) = await service.UpdateAsync(id, request);
            if (error is not null) return Results.BadRequest(new { error });
            return product is null ? Results.NotFound() : Results.Ok(product);
        }).RequireAuthorization("AdminOnly");

        group.MapPatch("/{id:int}", async (int id, UpdateProductRequest request, ProductService service) =>
        {
            var (product, error) = await service.UpdateAsync(id, request);
            if (error is not null) return Results.BadRequest(new { error });
            return product is null ? Results.NotFound() : Results.Ok(product);
        }).RequireAuthorization("AdminOnly");

        group.MapDelete("/{id:int}", async (int id, ProductService service) =>
        {
            var (success, error) = await service.DeleteAsync(id);
            if (error is not null) return Results.BadRequest(new { error });
            return success ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization("AdminOnly");

        group.MapPatch("/{id:int}/stock", async (int id, AdjustStockRequest request, ProductService service) =>
        {
            var (product, error) = await service.AdjustStockAsync(id, request.Adjustment);
            if (error is not null) return Results.BadRequest(new { error });
            return product is null ? Results.NotFound() : Results.Ok(product);
        }).RequireAuthorization("AdminOnly");
    }
}
