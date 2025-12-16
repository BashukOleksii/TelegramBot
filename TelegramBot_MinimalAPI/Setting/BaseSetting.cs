using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.Setting
{
    public class BaseSetting
    {
        [BsonElement("userId")] public long userId { get; set; }
        [BsonElement("latitude")] public float Latitude { get; set; }
        [BsonElement("longitude")] public float Longtitude { get; set; }
        [BsonElement("timezone")] public string TimeZone { get; set; } = "auto";
        [BsonElement("forecast_days")] public int? ForecastDays { get; set; } = null; // 1 3 9 14 16
        [BsonElement("past_days")] public int? PastDays { get; set; } = null; // 0 1 2 3 5 7 14 31 61
        [BsonElement("temperature_unit")] public string? TempUnit { get; set; } = null; //fahrenheit
        [BsonElement("wind_speed_10m")] public string? WindSpeed { get; set; } = null; //ms

        [BsonElement("curent")] public CurentWeatherSetting CurentSetting { get; set; } = new CurentWeatherSetting();
        [BsonElement("hourly")] public HourlyWeatherSetting HourlySetting { get; set; } = new HourlyWeatherSetting();
        [BsonElement("daily")] public DailyWeatherSetting DailySetting { get; set; } = new DailyWeatherSetting();

    }
}
