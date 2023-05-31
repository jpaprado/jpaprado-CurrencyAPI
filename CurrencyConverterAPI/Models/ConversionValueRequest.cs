using System;
namespace CurrencyConverterAPI.Models
{
	public class ConversionValueRequest
	{
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Value { get; set; }
    }
}

