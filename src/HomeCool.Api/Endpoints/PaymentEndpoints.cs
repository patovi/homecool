using HomeCool.Api.DTOs.Payments;
using HomeCool.Api.Services;

namespace HomeCool.Api.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments").WithTags("Payments").RequireAuthorization();

        group.MapGet("/", async (PaymentService service, int? userId) =>
            Results.Ok(await service.GetAllAsync(userId)));

        group.MapPost("/", async (CreatePaymentRequest request, PaymentService service) =>
        {
            var (payment, error) = await service.CreateAsync(request);
            return error is not null
                ? Results.BadRequest(new { error })
                : Results.Created($"/api/payments/{payment!.Id}", payment);
        }).RequireAuthorization("AdminOnly");
    }
}
