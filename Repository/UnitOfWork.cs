using ABCStoreAPI.Database;

namespace ABCStoreAPI.Repository;

public interface IUnitOfWork : IDisposable
{
    IExchangeRateRepository ExchangeRates { get; }
    int Complete();
    Task<int> CompleteAsync();
}
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IExchangeRateRepository ExchangeRates { get; private set; }

    public UnitOfWork(AppDbContext context, IExchangeRateRepository exchangeRateRepository)
    {
        _context = context;
        ExchangeRates = exchangeRateRepository;
    }

    public int Complete()
    {
        return _context.SaveChanges();
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
