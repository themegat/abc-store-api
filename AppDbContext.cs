using Microsoft.EntityFrameworkCore;

namespace ABCStoreAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Model.Product> Products { get; set; }
}
