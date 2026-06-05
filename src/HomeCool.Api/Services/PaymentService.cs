using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Payments;
using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Services;

public class PaymentService(AppDbContext db)
{
    public async Task<List<PaymentDto>> GetAllAsync(int? userId = null)
    {
        var query = db.Payments.Include(p => p.User).AsQueryable();
        if (userId.HasValue) query = query.Where(p => p.UserId == userId.Value);
        return await query
            .OrderByDescending(p => p.PaidAt)
            .Select(p => new PaymentDto(p.Id, p.UserId, p.User.Name, p.Amount, p.PaidAt, p.Note))
            .ToListAsync();
    }

    public async Task<(PaymentDto? Payment, string? Error)> CreateAsync(CreatePaymentRequest request)
    {
        var user = await db.Users.FindAsync(request.UserId);
        if (user is null || !user.IsActive)
            return (null, "Benutzer nicht gefunden oder inaktiv.");

        if (request.Amount <= 0)
            return (null, "Betrag muss größer als 0 sein.");

        var payment = new Payment
        {
            UserId = request.UserId,
            Amount = request.Amount,
            Note = request.Note
        };

        db.Payments.Add(payment);
        await db.SaveChangesAsync();
        return (new PaymentDto(payment.Id, user.Id, user.Name, payment.Amount, payment.PaidAt, payment.Note), null);
    }
}
