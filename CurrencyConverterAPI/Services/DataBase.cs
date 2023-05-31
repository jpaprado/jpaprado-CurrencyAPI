
using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CurrencyConverterAPI.Models;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace CurrencyConverterAPI.Services
{
    public class DataBase
    {
        private readonly IMongoCollection<CurrencyRate> _conversionsCollection;
        private readonly ExchangeRatesApiService _exchangeRatesApiService;


        ExchangeRatesApiService apiService = new ExchangeRatesApiService();


        public DataBase(
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

        public async Task<CurrencyRateInfo> GetNewCurrencyRate(string currency)
        {
            var filter = Builders<CurrencyRate>.Filter.Eq(x => x.Currency, currency);
            var sort = Builders<CurrencyRate>.Sort.Descending(x => x.LastUpdated);
            var currencyRate = await _conversionsCollection.Find(filter)
                .Sort(sort)
                .FirstOrDefaultAsync();

            if (currencyRate != null && IsWithinTimeLimit(currencyRate.LastUpdated))
            {
                var currencyRateInfo = new CurrencyRateInfo
                {
                    ExchangeRate = currencyRate.ExchangeRate,
                    LastUpdated = currencyRate.LastUpdated,
                };
                return currencyRateInfo;
            }

            return null;
        }



        public async Task<List<CurrencyRate>> GetAsync() =>
            await _conversionsCollection.Find(_ => true).ToListAsync();

        public async Task<CurrencyRate?> GetAsync(string id) =>
            await _conversionsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(CurrencyRate newCurrencyRate) =>
            await _conversionsCollection.InsertOneAsync(newCurrencyRate);

        public async Task UpdateAsync(string id, CurrencyRate updatedBook) =>
            await _conversionsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _conversionsCollection.DeleteOneAsync(x => x.Id == id);

        private bool IsWithinTimeLimit(DateTime lastUpdated)
        {
            var timeDifference = DateTime.UtcNow - lastUpdated;
            return timeDifference.TotalHours <= 12;
        }
    }

}



