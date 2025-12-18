using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.Setting
{
    public class CurentWeatherSetting
    {
        [BsonElement("temperature_2m")]
        public bool Temperature { get; set; } = true;

        [BsonElement("relative_humidity_2m")]
        public bool RelativeHumidity { get; set; } = true;

        [BsonElement("apparent_temperature")]
        public bool ApperentTemperature { get; set; } = true;

        [BsonElement("wind_speed_10m")]
        public bool WindSpeed { get; set; } = true;

        [BsonElement("wind_direction_10m")]
        public bool WindDirection { get; set; } = true;

        [BsonElement("precipitation")]
        public bool Precipitation { get; set; } = true; // Загальна кількість опадів мм

        [BsonElement("rain")]
        public bool Rain { get; set; } // Кількість дощу мм

        [BsonElement("snowfall")]
        public bool SnowFall { get; set; } // Кількість снігу мм

        [BsonElement("surface_pressure")]
        public bool SurfacePressure { get; set; } = true;

        [BsonElement("cloud_cover")]
        public bool CloudCover { get; set; } = true;

    }
}
