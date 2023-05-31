using CurrencyConverterAPI.Models;
using System.Text.Json;

public class ExchangeRatesApiService
{
    private readonly HttpClient _httpClient;
    private string apiKey;

    public ExchangeRatesApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.apilayer.com/");
        apiKey = "NNIMTJEEkyAWapZQ7MIXUz7RKruStce6";
    }

    // Convert currency asynchronously
    public virtual async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount, string date)
    {
        var requestUrl = _httpClient.BaseAddress + $"currency_data/convert?to={toCurrency}&from={fromCurrency}&amount={amount}&date={date}&apikey={apiKey}";
        Console.WriteLine($"requestUrl: {requestUrl}");

        var response = await _httpClient.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(result);

            var quoteValue = jsonDocument.RootElement
                .GetProperty("result")
                .GetDecimal();

            return quoteValue;
        }
        else
        {
            Console.WriteLine("Failed to retrieve currency conversion.");
            return -1;
        }
    }

    // Get historical currency rates asynchronously
    public async Task<ExchangeRatesApiResponse> CurrencyHistAsync(string date)
    {
        var requestUrl = _httpClient.BaseAddress + $"currency_data/historical?&date={date}&source=EUR&apikey={apiKey}";

        Console.WriteLine($"requestUrl: {requestUrl}");

        var response = await _httpClient.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();

            ExchangeRatesApiResponse responseJson = JsonSerializer.Deserialize<ExchangeRatesApiResponse>(result);

            var modifiedQuotes = new Dictionary<string, decimal>();

            foreach (var pair in responseJson.Quotes)
            {
                var currency = pair.Key.Substring(3);
                modifiedQuotes[currency] = pair.Value;
            }

            responseJson.Quotes = modifiedQuotes;

            Console.WriteLine($"quotesList {responseJson.Quotes}");

            return responseJson;
        }
        else
        {
            Console.WriteLine("Failed to retrieve currency conversion.");
            return null;
        }
    }

    // Get currency list asynchronously
    public async Task<string> CurrencyListAsync()
    {
        var requestUrl = _httpClient.BaseAddress + $"currency_data/list?apikey={apiKey}";

        Console.WriteLine($"requestUrl: {requestUrl}");

        var response = await _httpClient.GetStringAsync(requestUrl);

        return response;
    }
}
