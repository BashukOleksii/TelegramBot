using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.Response
{
    public class CurentWeatherResponse
    {

        [JsonPropertyName("temperature_2m")]
        public float? Temperature { get; set; } 

        [JsonPropertyName("relative_humidity_2m")]
        public float? RelativeHumidity { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public float? ApperentTemperature { get; set; } 

        [JsonPropertyName("wind_speed_10m")]
        public float? WindSpeed { get; set; } 

        [JsonPropertyName("wind_direction_10m")]
        public int? WindDirection { get; set; }

        [JsonPropertyName("precipitation")]
        public float? Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public float? Rain { get; set; } 

        [JsonPropertyName("snowfall")]
        public float? SnowFall { get; set; } 

        [JsonPropertyName("surface_pressure")]
        public float? SurfacePressure { get; set; } 

        [JsonPropertyName("cloud_cover")]
        public int? CloudCover { get; set; }

        public string GetInfo(string tempUnit, string speedUnit)
        {
            var stringRow = $"Поточна погода: {DateTime.Now:f}";

            if (Temperature is not null)
                stringRow += $"\nТемпература повітря: {Temperature} {tempUnit}";
            if (ApperentTemperature is not null)
                stringRow += $"\nВідчувається як {ApperentTemperature} {tempUnit}";
            if (RelativeHumidity is not null)
                stringRow += $"\nВідносна вологість: {RelativeHumidity} %";
            if (WindSpeed is not null)
                stringRow += $"\nШвидкість вітру: {WindSpeed} {speedUnit}";
            if (WindDirection is not null)
                stringRow += $"\nНапрямок вітру: {Direction.GetDirectionFromNumber(WindDirection)}";
            if (Precipitation is not null)
                stringRow += $"\nКількість опадів: {Precipitation} мм";
            if (Rain is not null)
                stringRow += $"\nКількість опадів від дощу: {Rain} мм";
            if (SnowFall is not null)
                stringRow += $"\nКількість опадів від снігу: {SnowFall} мм";
            if (SurfacePressure is not null)
                stringRow += $"\nАтмосферний тиск {(int)(SurfacePressure * 0.76006)} мм рт. ст.";
            if (CloudCover is not null)
                stringRow += $"\nВідсоток хмар на небі: {CloudCover}";


            return stringRow;
        }

       
    }
}
