using MongoDB.Bson.Serialization.IdGenerators;

namespace TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Interface
{
    public interface IWeatherDataService
    {
        Task<WeatherDataEntity?> GetData(long userID);
        Task SetHourlyData(WeatherDataEntity weatherDataEntity);
        Task<string?> GetPage(long userId, string propertyName, int index);
    }
}
