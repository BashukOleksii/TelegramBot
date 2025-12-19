using MongoDB.Driver;
using TelegramBot_MinimalAPI.MongoDB.State;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Interface;


namespace TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Realization
{
    public class WeatherDataRepository : IWeatherDataRepository
    {
        private readonly IMongoCollection<WeatherDataEntity> _collection;
        public WeatherDataRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<WeatherDataEntity>("WeatherData");
        }

        public async Task CreateAsync(WeatherDataEntity weatherData) =>
            await _collection.InsertOneAsync(weatherData);


        public async Task DeleteAsync(long userId) =>
            await _collection.DeleteOneAsync(e => e.UserId == userId);


        public async Task<WeatherDataEntity?> GetAsync(long userID) =>
            await _collection.Find(e => e.UserId == userID).FirstOrDefaultAsync();

        public async Task UpdateAsync(WeatherDataEntity weatherData) =>
            await _collection.ReplaceOneAsync(e => e.UserId == weatherData.UserId, weatherData);
        
    }
}
