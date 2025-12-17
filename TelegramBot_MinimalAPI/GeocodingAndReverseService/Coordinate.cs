using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.GeocodingAndReverseService
{
    public class GeocodingResult
    {
        [JsonPropertyName("results")] public List<Coordinate?>? results { get; set; }
    }
    public class Coordinate
    {
        [JsonPropertyName("latitude")] public float Latitude { get; set; }
        [JsonPropertyName("longitude")] public float Longitude { get; set; }
    }
}
