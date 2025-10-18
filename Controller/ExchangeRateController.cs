using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCStoreAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly Service.ExchangeRateService _exchangeRateService;

        public ExchangeRateController(Service.ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<Service.DataTransfer.ExchangeRateDto>>> GetAllExchangeRates()
        {
            var exchangeRates = await _exchangeRateService.GetAllExchangeRatesAsync();
            return Ok(exchangeRates);
        }

        [HttpGet]
        [Route("{currencyCode}")]
        public async Task<ActionResult<Service.DataTransfer.ExchangeRateDto>> GetExchangeRateByCurrencyCode(string currencyCode)
        {
            var exchangeRate = await _exchangeRateService.GetExchangeRateByCurrencyCodeAsync(currencyCode);
            if (exchangeRate == null)
            {
                return NotFound();
            }
            return Ok(exchangeRate);
        }
    }
}
