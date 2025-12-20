namespace TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Repository.Interface
{
    public interface IWeatherCacheRepository
    {
        Task<WeatherCache?> GetAsync(long userId);
        Task CreateAsync(WeatherCache weatherCache);
        Task UpdateAsync(WeatherCache weatherCache);
        Task DeleteAsync(long userId);
    }
}
