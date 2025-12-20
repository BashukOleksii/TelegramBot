using MongoDB.Bson.Serialization.IdGenerators;
using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.GeocodingAndReverseService.GetName
{
    public class NomanatimResponse
    {
        [JsonPropertyName("address")]
        public Adress Adress { get; set; }
    }
}
