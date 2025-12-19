using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.Response
{
    public class BaseResponse
    {
        [JsonPropertyName("current")]
        public CurentWeatherResponse? CurentWeather { get; set; }

        [JsonPropertyName("hourly")]
        public HourlyWeatherResponse? HourlyWeather { get; set; }

        [JsonPropertyName("daily")]
        public DailyWeatherResponse? DailyWeather { get; set; }

        public Dictionary<string,string?> GetInfo(string tempUnit, string speedUnit)
        {
            var data = new Dictionary<string, string?>();

            data["current"] = CurentWeather == null? null : CurentWeather.GetInfo(tempUnit, speedUnit);
            data["hourly"] = HourlyWeather == null ? null : HourlyWeather.GetInfo(tempUnit, speedUnit);
            data["daily"] = DailyWeather == null ? null : DailyWeather.GetInfo(tempUnit, speedUnit);

            return data;
        }
    }
}
