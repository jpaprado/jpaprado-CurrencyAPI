using System;
namespace CurrencyConverterAPI.Models
{
    public class CurrentDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ConversionsCollectionName { get; set; } = null!;
    }
}

