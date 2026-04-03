using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) :base(options){}
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().ToTable(Consts.PRODUCTS_TABLE);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName(Consts.ID).IsRequired();
            entity.Property(e => e.OwnerId).HasColumnName(Consts.OWNER_ID).IsRequired();
            entity.Property(e => e.Name).HasColumnName(Consts.NAME);
            entity.Property(e => e.Description).HasColumnName(Consts.DESCRIPTION);
            entity.Property(e => e.Category).HasColumnName(Consts.CATEGORY).IsRequired();
            entity.Property(e => e.SubCategory).HasColumnName(Consts.SUBCATEGORY);
            entity.Property(e => e.Price).HasColumnName(Consts.PRICE).IsRequired();
            entity.Property(e => e.Quantity).HasColumnName(Consts.QUANTITY);
        });
        
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
        
        modelBuilder.Entity<User>().ToTable(Consts.USERS_TABLE);
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName(Consts.ID).IsRequired();
            entity.Property(e => e.UserName).HasColumnName(Consts.NAME);
            entity.Property(e => e.Age).HasColumnName(Consts.AGE);
            entity.Property(e => e.Gender).HasColumnName(Consts.GENDER);
            entity.Property(e => e.City).HasColumnName(Consts.CITY);
            entity.Property(e => e.Country).HasColumnName(Consts.COUNTRY);
            entity.Property(e => e.CreatedAt).HasColumnName(Consts.CREATED_AT).IsRequired();
        });
        
        modelBuilder.Entity<Discount>().ToTable(Consts.DISCOUNTS_TABLE);
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.Property(e => e.DiscountId).HasColumnName(Consts.ID).IsRequired();
            entity.Property(e => e.Products).HasColumnName(Consts.PRODUCTS).HasColumnType("jsonb");
            entity.Property(e => e.Percentage).HasColumnName(Consts.PERCENTAGE).IsRequired();
            entity.Property(e => e.ExpiredAt).HasColumnName(Consts.EXPIRED_AT).IsRequired();
        });

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName().ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLower());
            }
        }
    }
}