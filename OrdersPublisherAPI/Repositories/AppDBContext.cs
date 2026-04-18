using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options){}
    
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Order>().ToTable(Consts.ORDERS_TABLE);
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName(Consts.ID).IsRequired();
            entity.Property(e => e.Timestamp).HasColumnName(Consts.TIMESTAMP).IsRequired();
            entity.ComplexProperty(e => e.Customer, b => b.ToJson(Consts.CUSTOMER_ID));
            entity.Property(e => e.TotalAmount).HasColumnName(Consts.TOTAL_AMOUNT);
            entity.Property(e => e.PaymentMethod).HasColumnName(Consts.PAYMENT_METHOD).IsRequired();
            entity.Property(e => e.DiscountApplied).HasColumnName(Consts.DISCOUNT_APPLIED).IsRequired();
            entity.ComplexProperty(e => e.Items, b => b.ToJson(Consts.ITEMS));
        });
    }
}