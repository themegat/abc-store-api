using ABCStoreAPI.Database;
using ABCStoreAPI.Database.Model;

namespace ABCStoreAPI.Repository;

public interface IExchangeRateRepository: IGenericRepository<ExchangeRate>
{   
}

public class ExchangeRateRepository: GenericRepository<ExchangeRate>, IExchangeRateRepository 
{
    public ExchangeRateRepository(AppDbContext context) : base(context)
    {
    }
}
