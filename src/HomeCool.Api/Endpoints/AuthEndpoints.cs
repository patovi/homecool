using System.Security.Claims;
using HomeCool.Api.DTOs.Auth;
using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest request, AuthService service) =>
        {
            var result = await service.LoginAsync(request);
            return result is null
                ? Results.Unauthorized()
                : Results.Ok(result);
        }).AllowAnonymous();

        group.MapPost("/logout", () => Results.Ok(new { message = "Logged out." }))
            .RequireAuthorization();

        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = user.FindFirstValue(ClaimTypes.Name);
            var role = user.FindFirstValue(ClaimTypes.Role);
            return Results.Ok(new { id = int.Parse(id!), name, role });
        }).RequireAuthorization();
    }
}
