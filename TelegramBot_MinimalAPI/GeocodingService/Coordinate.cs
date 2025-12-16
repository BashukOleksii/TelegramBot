using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.GeocodingService
{
    public class Coordinate
    {
        [JsonPropertyName("latitude")] public float Latitude { get; set; }
        [JsonPropertyName("longitude")] public float Longitude { get; set; }
    }
}
