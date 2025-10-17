namespace ABCStoreAPI.Service.DataTransfer;

public class ExchangeRateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }

    public static ExchangeRateDto toDto(Database.Model.ExchangeRate exchangeRate)
    {
        return new ExchangeRateDto
        {
            Code = exchangeRate.SupportedCurrency.Code,
            Name = exchangeRate.SupportedCurrency.Name,
            Rate = exchangeRate.Rate
        };
    }
}
