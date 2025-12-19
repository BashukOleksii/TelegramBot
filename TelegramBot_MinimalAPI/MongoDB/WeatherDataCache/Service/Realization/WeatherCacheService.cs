using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Service.Interface;

namespace TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Service.Realization
{
    public class WeatherCacheService : IWeatherCacheService
    {
        private readonly IWeatherCacheRepository _cacheRepository;

        public WeatherCacheService(IWeatherCacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task<WeatherCache?> Get(long userId) =>
            await _cacheRepository.GetAsync(userId);
        

        public async Task Send(WeatherCache weatherCache)
        {
            var cache = await _cacheRepository.GetAsync(weatherCache.UserId);

            if (cache is null)
                await _cacheRepository.CreateAsync(weatherCache);
            else
                await _cacheRepository.UpdateAsync(weatherCache);

        }
    }
}
