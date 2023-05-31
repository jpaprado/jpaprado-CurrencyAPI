using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CurrencyConverterAPI.Models
{
	public class ConversionValue
	{
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Resulte { get; set; }
        public decimal BaseValue { get; set; }
    }
}



