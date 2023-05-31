using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;
using FakeItEasy;
using Microsoft.Extensions.Options;
using Xunit;

namespace CurrencyConverterAPI.Tests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task GetCurrencyRate_ReturnsCurrencyRateInfo()
        {
            Console.WriteLine("TEST");

            // Arrange
            string currency = "USD";
            var existingCurrencyRate = new CurrencyRate
            {
                Currency = currency,
                ExchangeRate = 1.2m,
                LastUpdated = DateTime.UtcNow.AddHours(-6)
            };

            // Create a fake instance of IOptions<CurrentDatabaseSettings>
            var fakeOptions = A.Fake<IOptions<CurrentDatabaseSettings>>();
            var fakeOptionsValue = new CurrentDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "Currencies",
                ConversionsCollectionName = "Conversions"
            };
            A.CallTo(() => fakeOptions.Value).Returns(fakeOptionsValue);

            // Create a fake instance of ExchangeRatesApiService
            var fakeExchangeRatesApiService = A.Fake<ExchangeRatesApiService>();
            A.CallTo(() => fakeExchangeRatesApiService.ConvertCurrencyAsync(currency, A<string>._, A<decimal>._, A<string>._)).Returns(Task.FromResult(1.2m));

            // Create an instance of CurrencyService with the fake instances
            var currencyService = new CurrencyService(fakeOptions, fakeExchangeRatesApiService);

            // Act
            var result = await currencyService.GetCurrencyRate(currency);

            Console.WriteLine($"result: {result.ExchangeRate}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingCurrencyRate.ExchangeRate, result.ExchangeRate);
            Assert.Equal(existingCurrencyRate.LastUpdated, result.LastUpdated);
        }
    }
}
