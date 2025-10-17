using System;
using ABCStoreAPI.Configuration;
using ABCStoreAPI.Database;
using ABCStoreAPI.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABCStoreAPI.Service;

class ExchangeRateResponse
{
    [JsonProperty("time_last_update_unix")]
    public long TimeLastUpdateUnix { get; set; }

    [JsonProperty("time_last_update_utc")]
    public string TimeLastUpdateUtc { get; set; } = string.Empty;

    [JsonProperty("time_next_update_unix")]
    public long TimeNextUpdateUnix { get; set; }

    [JsonProperty("time_next_update_utc")]
    public string TimeNextUpdateUtc { get; set; } = string.Empty;

    [JsonProperty("base_code")]
    public string BaseCode { get; set; } = string.Empty;

    [JsonProperty("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; set; } = new Dictionary<string, decimal>();
}

public class ExchangeRateImportService
{
    private readonly ILogger<ExchangeRateImportService> _logger;
    private const string SysUser = "system";
    private readonly HttpClient _httpClient;

    private readonly string _apiKey;
    private readonly string _baseCurrency;
    private readonly string _apiUrl;
    private readonly AppDbContext _dbContext;

    public ExchangeRateImportService(HttpClient httpClient,
    ILogger<ExchangeRateImportService> logger, IOptions<ApiConfig> apiConfig,
    AppDbContext dbContext)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = apiConfig.Value.ExchangeRate.ApiKey;
        _baseCurrency = apiConfig.Value.ExchangeRate.BaseCurrency;
        _apiUrl = apiConfig.Value.ExchangeRate.Url;
        _dbContext = dbContext;
    }

    private async Task PersistExchangeRates(ExchangeRateResponse exchangeRateResponse)
    {
        int count = 0;

        var supportedCurrencies = await _dbContext.SupportedCurrency
            .ToListAsync();

        foreach (var currency in supportedCurrencies)
        {
            var exchangeRates = exchangeRateResponse.ConversionRates.ToList();
            var rateEntry = exchangeRates.FindAll(rateEntry => rateEntry.Key == currency.Code);

            if (rateEntry != null)
            {
                var exchangeRate = new ExchangeRate
                {
                    TimeLastUpdateUnix = exchangeRateResponse.TimeLastUpdateUnix,
                    TimeNextUpdateUnix = exchangeRateResponse.TimeNextUpdateUnix,
                    Rate = rateEntry.First().Value,
                    SupportedCurrencyId = currency.Id,
                    CreatedBy = SysUser,
                    UpdatedBy = SysUser
                };

                _dbContext.ExchangeRate.Add(exchangeRate);
                count++;
            }
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Imported {Count} exchange rates.", count);
    }

    private async Task<bool> TruncateExchangeRates()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(ExchangeRate));
        if (entityType == null)
        {
            return false;
        }
        else
        {
            var tableName = entityType.GetTableName();
            string sql = $"TRUNCATE TABLE \"{tableName}\"";
            await _dbContext.Database.ExecuteSqlRawAsync(sql);
            return true;
        }
    }

    public async Task RunExchangeRateImportAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/{_apiKey}/latest/{_baseCurrency}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var exchangeRateResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(content);

        if (exchangeRateResponse != null)
        {
            if(await TruncateExchangeRates())
            {
                await PersistExchangeRates(exchangeRateResponse);
            }
            else
            {
                _logger.LogError("Failed to truncate ExchangeRate table, skipping import.");
            }
        }
    }
}
