namespace TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Interface
{
    public interface IWeatherDataRepository
    {
        Task CreateAsync(WeatherDataEntity weatherData);
        Task<WeatherDataEntity?> GetAsync(long userID);
        Task DeleteAsync(long userId);
        Task UpdateAsync(WeatherDataEntity weatherData);
    }
}
