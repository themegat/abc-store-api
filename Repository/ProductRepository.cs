using ABCStoreAPI.Database;
using ABCStoreAPI.Database.Model;

namespace ABCStoreAPI.Repository;

public interface IProductRepository : IGenericRepository<Product>
{
    public IQueryable<Product> GetAllByCategory(int categoryId);
    public IQueryable<Product> SearchBy(string searchTerm);
}

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Product> GetAllByCategory(int categoryId)
    {
        return _dbSet.Where(p => p.ProductCategoryId == categoryId);
    }

    public IQueryable<Product> SearchBy(string searchTerm)
    {
        searchTerm = searchTerm.Trim().ToLower();
        return _dbSet.Where(p => p.Name.ToLower().Contains(searchTerm) || p.Description.ToLower().Contains(searchTerm));
    }
}
