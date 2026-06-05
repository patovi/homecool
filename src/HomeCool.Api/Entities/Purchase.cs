namespace HomeCool.Api.Entities;

public class Purchase
{
    public int Id { get; set; }
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal TotalCost { get; set; }
    public string? Note { get; set; }

    public ICollection<PurchaseItem> Items { get; set; } = [];
}
