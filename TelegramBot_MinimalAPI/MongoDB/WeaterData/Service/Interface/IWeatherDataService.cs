using MongoDB.Bson.Serialization.IdGenerators;

namespace TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Interface
{
    public interface IWeatherDataService
    {
        Task<string?> GetDate(int userIndex, int rowIndex, string propName);
        Task<WeatherDataEntity?> GetData(int userID);
        Task SetHourlyData(WeatherDataEntity weatherDataEntity);
    }
}
