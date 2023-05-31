using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CurrencyConverterAPI.Models
{
	public class CurrencyRate
    {
    [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; } = null!;

        [BsonElement("exchangeRate")]
        public decimal ExchangeRate { get; set; }

        [BsonElement("date")]
        public DateTime LastUpdated { get; set; }
    }
}

