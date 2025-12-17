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
                var url = $"https://geocoding-api.open-meteo.com/v1/search?name={name}&count=1&language=uk";

                var response = await _httpClient.GetAsync(url);

                var result = await response.Content.ReadFromJsonAsync<Coordinate>();

                if (result.Latitude == null || result.Longitude == null)
                    result = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<string> GetNameAsync(float lat, float lon)
        {
            var url = $"reverse?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&format=json&accept-language=uk";

            var response = await _httpClient.GetFromJsonAsync<NomanatimResponse>(url);

            if (response.Adress is null)
                return "Не знайдено";

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
