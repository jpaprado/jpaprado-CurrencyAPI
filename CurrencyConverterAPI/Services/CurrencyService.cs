using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CurrencyConverterAPI.Models;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace CurrencyConverterAPI.Services
{
    public class CurrencyService
    {
        private readonly IMongoCollection<CurrencyRate> _conversionsCollection;
        private readonly ExchangeRatesApiService _exchangeRatesApiService;



//ExchangeRatesApiService apiService = new ExchangeRatesApiService();


        public CurrencyService(
            IOptions<CurrentDatabaseSettings> currentDatabaseSettings,
            ExchangeRatesApiService exchangeRatesApiService)
        {
            var mongoClient = new MongoClient(
                currentDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                currentDatabaseSettings.Value.DatabaseName);

            _conversionsCollection = mongoDatabase.GetCollection<CurrencyRate>(
                currentDatabaseSettings.Value.ConversionsCollectionName);

            _exchangeRatesApiService = new ExchangeRatesApiService();
        }

        public async Task<CurrencyRateInfo> GetCurrencyRate(string currency)
        {
            var filter = Builders<CurrencyRate>.Filter.Eq(x => x.Currency, currency);
            var sort = Builders<CurrencyRate>.Sort.Descending(x => x.LastUpdated);
            var currencyRate = await _conversionsCollection.Find(filter)
                .Sort(sort)
                .FirstOrDefaultAsync();

            Console.WriteLine($"Currency {currency}");

            if (currencyRate != null && IsWithinTimeLimit(currencyRate.LastUpdated))
            {
                var currencyRateInfo = new CurrencyRateInfo
                {
                    ExchangeRate = currencyRate.ExchangeRate,
                    LastUpdated = currencyRate.LastUpdated,
                };
                return currencyRateInfo;
            }

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var value = await _exchangeRatesApiService.ConvertCurrencyAsync("EUR", currency, 1,today);

            if (value > 0)
            {
                var newCurrencyRate = new CurrencyRate
                {
                    ExchangeRate = value,
                    LastUpdated = DateTime.UtcNow,
                    Currency = currency
                };

                await CreateAsync(newCurrencyRate);
       
                var currencyRateInfo = new CurrencyRateInfo
                {
                    ExchangeRate = value,
                    LastUpdated = DateTime.UtcNow,
                };
                return currencyRateInfo;

            }


            return null;
        }

        public async Task<ExchangeRatesApiResponse> GetHistorical(int days)
        {
            
            String newDateTime = DateTime.UtcNow.AddDays(-days).ToString("yyyy-MM-dd");
            

            var list = await _exchangeRatesApiService.CurrencyHistAsync(newDateTime);

            Console.WriteLine($"list {list}"); 

            return list;
        }

        public async Task<CurrencyRateInfo> GetByCurrencyAsync(string currency)
        {
            var filter = Builders<CurrencyRate>.Filter.Eq(x => x.Currency, currency);
            var sort = Builders<CurrencyRate>.Sort.Descending(x => x.LastUpdated);
            var currencyRate = await _conversionsCollection.Find(filter)
                .Sort(sort)
                .FirstOrDefaultAsync();

            if (currencyRate != null)
            {
                var timeDifference = DateTime.UtcNow - currencyRate.LastUpdated;
                if (timeDifference.TotalHours > 12)
                {
                    return null; // Retorna null se a cotação foi armazenada há mais de 12 horas
                }

                var currencyRateInfo = new CurrencyRateInfo
                {
                    ExchangeRate = currencyRate.ExchangeRate,
                    LastUpdated = currencyRate.LastUpdated,
                };
                return currencyRateInfo;
            }

            return null;
        }

        

        public async Task<Dictionary<string, string>> CurrencyList()
        {

            var response = await _exchangeRatesApiService.CurrencyListAsync();

            Console.WriteLine($"response {response}");

            Dictionary<string, string> currencyList = new Dictionary<string, string>();

            JsonDocument document = JsonDocument.Parse(response);
            JsonElement currenciesElement = document.RootElement.GetProperty("currencies");

            foreach (JsonProperty property in currenciesElement.EnumerateObject())
            {
                string currencyCode = property.Name;
                string countryName = property.Value.GetString();
                currencyList.Add(currencyCode, countryName);
            }

            return currencyList;
        }

        public async Task<ConversionValue> GetConversioValuet(ConversionValueRequest conversionValueRequest)
        {

            var response = await _exchangeRatesApiService.CurrencyListAsync();

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var value = await _exchangeRatesApiService.ConvertCurrencyAsync(conversionValueRequest.FromCurrency, conversionValueRequest.ToCurrency, conversionValueRequest.Value, today);

            Console.WriteLine($"value: {value}");
            var conversionValue = new ConversionValue
            {
                FromCurrency = conversionValueRequest.FromCurrency,
                ToCurrency = conversionValueRequest.ToCurrency,
                Resulte = value,
                BaseValue = conversionValueRequest.Value
            };

            // Retorne o objeto ConversionValue criado
            return conversionValue;
        }

        public async Task CreateAsync(CurrencyRate newCurrencyRate) =>
            await _conversionsCollection.InsertOneAsync(newCurrencyRate);

        private bool IsWithinTimeLimit(DateTime lastUpdated)
        {
            var timeDifference = DateTime.UtcNow - lastUpdated;
            return timeDifference.TotalHours <= 12;
        }
    }
    
}

