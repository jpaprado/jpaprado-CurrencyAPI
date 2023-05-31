using System;
using System.Text.Json.Serialization;

namespace CurrencyConverterAPI.Models
{
	public class ExchangeRatesApiResponse
	{
        [JsonPropertyName("quotes")]
        public Dictionary<string, decimal> Quotes { get; set; }
    }
}

