using System;
namespace CurrencyConverterAPI.Models
{
    public class CurrencyRateInfo
    {
        public decimal ExchangeRate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

