using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) :base(options){}
    
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().ToTable(Consts.PRODUCTS_TABLE);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName(Consts.ID).IsRequired();
            entity.Property(e => e.Name).HasColumnName(Consts.NAME);
            entity.Property(e => e.Description).HasColumnName(Consts.DESCRIPTION);
            entity.Property(e => e.Category).HasColumnName(Consts.CATEGORY).IsRequired();
            entity.Property(e => e.SubCategory).HasColumnName(Consts.SUBCATEGORY);
            entity.Property(e => e.Price).HasColumnName(Consts.PRICE).IsRequired();
            entity.Property(e => e.Quantity).HasColumnName(Consts.QUANTITY);
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
    }
}