namespace TelegramBot_MinimalAPI.GeocodingService
{
    public class GeocodingServise
    {
        private readonly HttpClient _httpClient;

        public GeocodingServise(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Coordinate?> GetPoint(string name)
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
    }
}
