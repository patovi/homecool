using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Consumption> Consumptions => Set<Consumption>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<MonthlySettlement> MonthlySettlements => Set<MonthlySettlement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Name).IsUnique();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Price).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Purchase>(e =>
        {
            e.Property(p => p.TotalCost).HasColumnType("TEXT");
            e.HasOne(p => p.CreatedByUser).WithMany().HasForeignKey(p => p.CreatedByUserId);
        });

        modelBuilder.Entity<PurchaseItem>(e =>
        {
            e.Property(p => p.UnitCost).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Consumption>(e =>
        {
            e.Property(c => c.PriceAtTime).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.Property(p => p.Amount).HasColumnType("TEXT");
        });

        modelBuilder.Entity<MonthlySettlement>(e =>
        {
            e.Property(m => m.TotalConsumed).HasColumnType("TEXT");
            e.Property(m => m.TotalPaid).HasColumnType("TEXT");
            e.Property(m => m.Balance).HasColumnType("TEXT");
            e.HasIndex(m => new { m.UserId, m.Year, m.Month }).IsUnique();
        });
    }
}
