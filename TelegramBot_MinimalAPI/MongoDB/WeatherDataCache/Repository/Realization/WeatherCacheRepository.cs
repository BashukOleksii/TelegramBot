using MongoDB.Driver;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Repository.Interface;

namespace TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Repository.Realization
{
    public class WeatherCacheRepository: IWeatherCacheRepository
    {
        private readonly IMongoCollection<WeatherCache> _collection;

        public WeatherCacheRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<WeatherCache>("WeatherCache");
        }

        public async Task CreateAsync(WeatherCache weatherCache) =>
            await _collection.InsertOneAsync(weatherCache);


        public async Task DeleteAsync(long userId) =>
            await _collection.DeleteOneAsync(e => e.UserId == userId);


        public async Task<WeatherCache?> GetAsync(long userId) =>
            await _collection.Find(e => e.UserId == userId).FirstOrDefaultAsync();


        public async Task UpdateAsync(WeatherCache weatherCache) =>
            await _collection.ReplaceOneAsync(e => e.UserId == weatherCache.UserId, weatherCache);
        
    }
}
