namespace TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Service.Interface
{
    public interface IWeatherCacheService
    {
        Task<WeatherCache?> Get(long userId);
        Task Send(WeatherCache weatherCache);
    }
}
