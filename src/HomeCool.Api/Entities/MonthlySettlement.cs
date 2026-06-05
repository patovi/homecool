namespace HomeCool.Api.Entities;

public class MonthlySettlement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalConsumed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; } // negative = user owes money
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
