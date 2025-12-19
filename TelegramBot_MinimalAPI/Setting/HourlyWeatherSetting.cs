using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.Setting
{
    public class HourlyWeatherSetting
    {
        [BsonElement("temperature_2m")]
        public bool Temperature { get; set; } = true;
        [BsonElement("precipitation_probability")]
        public bool PrecipitationProbality { get; set; } = true;

        [BsonElement("visibility")]
        public bool Visibility { get; set; }

        [BsonElement("wind_speed_10m")]
        public bool WindSpeed { get; set; } = true;

        [BsonElement("wind_gusts_10m")]
        public bool WindGusts { get; set; } = true; //Пориви

    }
}


