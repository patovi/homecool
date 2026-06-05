using HomeCool.Api.DTOs.Users;
using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization();

        group.MapGet("/", async (UserService service) =>
            Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, UserService service) =>
        {
            var user = await service.GetByIdAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        group.MapPost("/", async (CreateUserRequest request, UserService service) =>
        {
            var (user, error) = await service.CreateAsync(request);
            return error is not null
                ? Results.BadRequest(new { error })
                : Results.Created($"/api/users/{user!.Id}", user);
        }).RequireAuthorization("AdminOnly");

        group.MapPut("/{id:int}", async (int id, UpdateUserRequest request, UserService service) =>
        {
            var (user, error) = await service.UpdateAsync(id, request);
            if (error is not null) return Results.BadRequest(new { error });
            return user is null ? Results.NotFound() : Results.Ok(user);
        }).RequireAuthorization("AdminOnly");

        group.MapPatch("/{id:int}", async (int id, UpdateUserRequest request, UserService service) =>
        {
            var (user, error) = await service.UpdateAsync(id, request);
            if (error is not null) return Results.BadRequest(new { error });
            return user is null ? Results.NotFound() : Results.Ok(user);
        }).RequireAuthorization("AdminOnly");
    }
}
