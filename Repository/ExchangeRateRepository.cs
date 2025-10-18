using ABCStoreAPI.Database;
using ABCStoreAPI.Database.Model;

namespace ABCStoreAPI.Repository;

public interface IExchangeRateRepository: IGenericRepository<ExchangeRate>
{
    public IQueryable<ExchangeRate> GetByCurrency(string currencyCode);
}

public class ExchangeRateRepository: GenericRepository<ExchangeRate>, IExchangeRateRepository 
{
    public ExchangeRateRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<ExchangeRate> GetByCurrency(string currencyCode)
    {
        currencyCode = currencyCode.Trim().ToUpper();
        return _dbSet.Where(er => er.SupportedCurrency.Code.ToUpper() ==  currencyCode);
    }
}
