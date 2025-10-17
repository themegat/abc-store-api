namespace ABCStoreAPI.Configuration;

public class ExchangeRateConfig
{
    public string ApiKey { get; set; } = "";
    public string BaseCurrency { get; set; } = "";
    public string Url { get; set; } = "";
}

public class ApiConfig
{
    public string ProductApiUrl { get; set; } = "";
    public ExchangeRateConfig ExchangeRate { get; set; } = new ExchangeRateConfig();
}
