using ABCStoreAPI.Database;
using ABCStoreAPI.Database.Model;

namespace ABCStoreAPI.Repository;

public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
{
}
public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(AppDbContext context) : base(context)
    {
    }

}
