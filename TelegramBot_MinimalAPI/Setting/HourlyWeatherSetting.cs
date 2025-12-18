using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.Setting
{
    public class HourlyWeatherSetting
    {
        [BsonElement("temperature_2m")]
        public bool Temperature { get; set; } = true;

        [BsonElement("relative_humidity_2m")]
        public bool RelativeHumidity { get; set; }

        [BsonElement("precipitation_probability")]
        public bool PrecipitationProbality { get; set; } = true;

        [BsonElement("visibility")]
        public bool Visibility { get; set; }

        [BsonElement("dew_point_2m")]
        public bool DewPoint { get; set; }

        [BsonElement("wind_speed_10m")]
        public bool WindSpeed { get; set; } = true;

        [BsonElement("wind_gusts_10m")]
        public bool WindGusts { get; set; } = true; //Пориви

        [BsonElement("wind_direction_10m")]
        public bool WindDirection { get; set; }

        [BsonElement("cloud_cover")]
        public bool CloudCover { get; set; }

    }
}


