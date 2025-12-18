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

        public List<string> GetInfo(string tempUnit, string speedUnit)
        {
            var info = new List<string>();

            if (CurentWeather is not null)
                info.Add(CurentWeather.GetInfo(tempUnit, speedUnit));
            if (HourlyWeather is not null)
                info.Add(HourlyWeather.GetInfo(tempUnit, speedUnit));
            if (DailyWeather is not null)
                info.Add(DailyWeather.GetInfo(tempUnit, speedUnit));

            return info;
        }
    }
}
