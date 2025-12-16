using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.Setting
{
    public class DailyWeatherSetting
    {
        [BsonElement("temperature_2m_max")]
        public bool MaxTemp { get; set; } = true;

        [BsonElement("temperature_2m_min")]
        public bool MinTemp { get; set; } = true;

        [BsonElement("apparent_temperature_max")]
        public bool ApperentMax { get; set; } = true;

        [BsonElement("apparent_temperature_min")]
        public bool ApperentMin { get; set; } = true;

        [BsonElement("wind_speed_10m_max")]
        public bool WindSpeedMax { get; set; } = true;

        [BsonElement("wind_direction_10m_dominant")]
        public bool DominantWindDirection { get; set; } = true;

        [BsonElement("sunrise")]
        public bool SunRise { get; set; } = true;

        [BsonElement("sunset")]
        public bool SunSet { get; set; } = true;
        
        [BsonElement("daylight_duration")]
        public bool DayLightDuration { get; set; } = true;

        [BsonElement("precipitation_sum")]
        public bool PrecipitatiobSum { get; set; } = true;

        [BsonElement("rain_sum")]
        public bool RainSum { get; set; } 

        [BsonElement("showers_sum")] // Зливи
        public bool ShowersSun { get; set; } 

        [BsonElement("snowfall_sum")] // Сніг
        public bool SnowdallSum { get; set; }
        
        [BsonElement("precipitation_hours")] 
        public bool PrecipitationHours { get; set; } 

    }
}
