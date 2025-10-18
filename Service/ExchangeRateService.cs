using ABCStoreAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace ABCStoreAPI.Service;

public class ExchangeRateService
{
    private readonly IUnitOfWork _uow;

    public ExchangeRateService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<DataTransfer.ExchangeRateDto>> GetAllExchangeRatesAsync()
    {
        var exchangeRates = await _uow.ExchangeRates
        .GetAll()
        .Include(e => e.SupportedCurrency)
        .ToListAsync();

        return exchangeRates.Select(DataTransfer.ExchangeRateDto.toDto).ToList();
    }

    public async Task<DataTransfer.ExchangeRateDto?> GetExchangeRateByCurrencyCodeAsync(string currencyCode)
    {
        var exchangeRate = await _uow.ExchangeRates
        .GetByCurrency(currencyCode)
        .Include(e => e.SupportedCurrency)
        .FirstOrDefaultAsync();

        if (exchangeRate == null)
        {
            return null;
        }

        return DataTransfer.ExchangeRateDto.toDto(exchangeRate);
    }
}
