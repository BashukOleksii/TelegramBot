using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.GeocodingAndReverseService.GetName
{
    public class Adress
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("state")]
        public string? State { get; set; }
        [JsonPropertyName("district")]
        public string? District { get; set; }
        [JsonPropertyName("village")]
        public string? Village { get; set; }

    }
}
