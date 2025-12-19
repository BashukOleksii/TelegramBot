using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace TelegramBot_MinimalAPI.Response
{
    public class DailyWeatherResponse
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; }
        
        [JsonPropertyName("temperature_2m_max")]
        public List<float>? MaxTemp { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<float>? MinTemp { get; set; }

        [JsonPropertyName("apparent_temperature_max")]
        public List<float>? ApperentMax { get; set; }

        [JsonPropertyName("apparent_temperature_min")]
        public List<float>? ApperentMin { get; set; }

        [JsonPropertyName("wind_speed_10m_max")]
        public List<float>? WindSpeedMax { get; set; }

        [JsonPropertyName("wind_direction_10m_dominant")]
        public List<int>? DominantWindDirection { get; set; }

        [JsonPropertyName("sunrise")]
        public List<string>? SunRise { get; set; } 

        [JsonPropertyName("sunset")]
        public List<string>? SunSet { get; set; }

        [JsonPropertyName("daylight_duration")]
        public List<float>? DayLightDuration { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public List<float>? PrecipitationSum { get; set; } 

        [JsonPropertyName("rain_sum")]
        public List<float>? RainSum { get;  set; }

        [JsonPropertyName("showers_sum")] 
        public List<float>? ShowersSum { get; set; }

        [JsonPropertyName("snowfall_sum")] 
        public List<float>? SnowdallSum { get; set; }

        public string GetInfo(string tempUnit, string speedUnit)
        {
            if (Time is null)
                return null;


            string stringRow = "Поденна погода:";

            for (int i = 0; i < Time.Count; i++)
            {
                stringRow += $"\n<b>#####{DateTime.Parse(Time[i]):D}</b>";

                if (MaxTemp is not null)
                    stringRow += $"\n\t- Максимальна температура повітря: {MaxTemp[i]} {tempUnit}";
                if (MinTemp is not null)
                    stringRow += $"\n\t- Мінімальна температура повітря: {MinTemp[i]} {tempUnit}";
                if (ApperentMax is not null)
                    stringRow += $"\n\t- Максимальна температура відчувається як {ApperentMax[i]} {tempUnit}";
                if (ApperentMin is not null)
                    stringRow += $"\n\t- Мінімальна температура відчувається як {ApperentMin[i]} {tempUnit}";
                if (WindSpeedMax is not null)
                    stringRow += $"\n\t- Максимальна швидкість вітру {WindSpeedMax[i]} {speedUnit}";
                if (DominantWindDirection is not null)
                    stringRow += $"\n\t- Переважний напрямок вітру {Direction.GetDirectionFromNumber(DominantWindDirection[i])}";
                if (SunRise is not null)
                    stringRow += $"\n\t- Схід сонця о {DateTime.Parse(SunRise[i]):t}";
                if (SunSet is not null)
                    stringRow += $"\n\t- Захід сонця о {DateTime.Parse(SunSet[i]):t}";
                if (DayLightDuration is not null)
                    stringRow += $"\n\t- Тривалість дня {(int)DayLightDuration[i]/3600} год {(int)(DayLightDuration[i]%3600)/60} хв";
                if (PrecipitationSum is not null)
                    stringRow += $"\n\t- Кількість опадів {PrecipitationSum[i]} мм";
                if (RainSum is not null)
                    stringRow += $"\n\t- Кількість опадів від дощу: {RainSum[i]} мм";
                if (ShowersSum is not null)
                    stringRow += $"\n\t- Кількість опадів від зливи: {ShowersSum[i]} мм";
                if (SnowdallSum is not null)
                    stringRow += $"\n\t- Кількість опадів від снігу: {SnowdallSum[i]} мм";

                if (i < Time.Count - 1)
                    stringRow += "_____";

            }

            return stringRow;
        }
    }
}
