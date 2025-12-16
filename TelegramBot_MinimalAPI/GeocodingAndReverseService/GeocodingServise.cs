using System.Text.Json;

namespace TelegramBot_MinimalAPI.GeocodingAndReverseService
{
    public class GeocodingServise
    {
        private readonly HttpClient _httpClient;

        public GeocodingServise(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Coordinate?> GetPointAsync(string name)
        {
            try
            {
                var url = $"https://geocoding-api.open-meteo.com/v1/search?name={name}&count=1&language=uk";

                var response = await _httpClient.GetAsync(url);

                var result = await response.Content.ReadFromJsonAsync<Coordinate>();
                
                if(result.Latitude == null || result.Longitude == null)
                    result = null;

                return result;
            }
            catch
            {
                return null;
            }
        } 
        public async Task<string> GetNameAsync(float lat, float lon)
        {
            var url = $"https://geocoding-api.open-meteo.com/v1/reverse?latitude={lat}&longitude={lon}&language=uk";

            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);

            var results = doc.RootElement.GetProperty("result");

            if(results.GetArrayLength() > 0)
            {
                var first = results[0];

                return first.GetProperty("name").GetString() ?? "Не знайдено";
            }

            return "Не знайдено";

                
        } 
    }
}
