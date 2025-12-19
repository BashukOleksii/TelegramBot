using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.Response
{
    public class HourlyWeatherResponse
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; }
        [JsonPropertyName("temperature_2m")]
        public List<float>? Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public List<float>? RelativeHumidity { get; set; }

        [JsonPropertyName("precipitation_probability")]
        public List<float>? PrecipitationProbality { get; set; } 

        [JsonPropertyName("visibility")]
        public List<float>? Visibility { get; set; }

        [JsonPropertyName("dew_point_2m")]
        public List<float>? DewPoint { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public List<float>? WindSpeed { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public List<float>? WindGusts { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public List<int?>? WindDirection { get; set; }

        [JsonPropertyName("cloud_cover")]
        public List<int>? CloudCover { get; set; }

        public string GetInfo(string tempUnit, string speedUnit)
        {
            if (Time is null)
                return null;

            string date = DateTime.Parse(Time[0]).ToString("D");

            var stringRow = $"Погодинна погода:";
            stringRow += $"\n<b>#####({date})#####</b>";
            for (int i = 0; i < Time.Count; i++)
            {
                if(date != DateTime.Parse(Time[i]).ToString("D"))
                {
                    stringRow += "_____";
                    date = DateTime.Parse(Time[i]).ToString("D");
                    stringRow += $"<b>#####({date})#####</b>";
                }

                stringRow += $"\n<b>*****{DateTime.Parse(Time[i]).ToString("t")}:</b>";

                if (Temperature is not null)
                    stringRow += $"\n\t-Температура повітря: {Temperature[i]} {tempUnit}";
                if (PrecipitationProbality is not null)
                    stringRow += $"\n\t -Ймовірність дощу {PrecipitationProbality[i]} %";
                if (RelativeHumidity is not null)
                    stringRow += $"\n\t -Відносна вологість: {RelativeHumidity[i]} %";
                if (WindSpeed is not null)
                    stringRow += $"\n\t -Швидкість вітру: {WindSpeed[i]} {speedUnit}";
                if (WindDirection is not null)
                    stringRow += $"\n\t -Напрямок вітру: {Direction.GetDirectionFromNumber(WindDirection[i])}";
                if (WindGusts is not null)
                    stringRow += $"\n\t -Пориви вітру: {WindGusts[i]} {speedUnit}";
                if (DewPoint is not null)
                    stringRow += $"\n\t -Точка роси: {DewPoint[i]} {tempUnit}";
                if (Visibility is not null)
                    stringRow += $"\n\t -Видимість: {Visibility[i]} м";
                if (CloudCover is not null)
                    stringRow += $"\n\t -Відсоток хмар на небі: {CloudCover[i]}";
            }

            return stringRow;
        }
    }
}
