using MongoDB.Driver.Core.Misc;
using System.Globalization;
using System.Text.Json;
using TelegramBot_MinimalAPI.GeocodingAndReverseService.GetName;

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
                var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(name)}&count=1&language=uk";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<GeocodingResult>();

                if (result?.results is null || result.results.Count == 0)
                    return null;

                return result.results[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<string?> GetNameAsync(float lat, float lon)
        {
            var url = $"reverse?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&format=json&accept-language=uk";

            var response = await _httpClient.GetFromJsonAsync<NomanatimResponse>(url);

            if (response.Adress is null)
                return null;

            string location = "";

            if (response.Adress.State is not null)
                location += response.Adress.State + " ";
            if (response.Adress.District is not null)
                location += response.Adress.District + " ";
            if (response.Adress.City is not null)
                location += response.Adress.City + " ";
            if (response.Adress.Village is not null)
                location += response.Adress.Village + " ";

            return location;

        }
    }
}
