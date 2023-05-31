using System;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;

namespace CurrencyConverterAPI.Controllers
{
    [ApiController]
    [Route("/api/currencies")]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _currencyService;

        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> Get() =>
            await _currencyService.CurrencyList();
        //public async Task<List<CurrencyRate>> Get() =>


        [HttpGet("historic/{days}")]
        public async Task<ActionResult<ExchangeRatesApiResponse>> GetHistorical(int days)
        {

            var currencyRateInfo = await _currencyService.GetHistorical(days);
            
            if (currencyRateInfo == null)
            {
                return NotFound();
            }

            return currencyRateInfo;
        }

        [HttpGet("{baseCurrency}")]
        public async Task<ActionResult<CurrencyRateInfo>> GetCurrencyRate(string baseCurrency)
        {

            var currencyRateInfo = await _currencyService.GetCurrencyRate(baseCurrency);

            if (currencyRateInfo == null)
            {
                return NotFound();
            }

            return currencyRateInfo;
        }

        [HttpPost("api/currencies")]
        public async Task<ConversionValue> Post([FromBody] ConversionValueRequest conversionValueRequest)
        {
            var conversionValue = await _currencyService.GetConversioValuet(conversionValueRequest);

            // Retorne o objeto ConversionValue criado
            return conversionValue;
        }





    }
}
