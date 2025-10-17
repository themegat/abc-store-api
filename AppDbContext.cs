using ABCStoreAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace ABCStoreAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<ProductCategory> ProductCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategory)
            .WithMany()
            .HasForeignKey(p => p.ProductCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
