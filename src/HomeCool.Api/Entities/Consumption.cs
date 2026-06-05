namespace HomeCool.Api.Entities;

public class Consumption
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal PriceAtTime { get; set; } // historical price snapshot
    public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;
}
