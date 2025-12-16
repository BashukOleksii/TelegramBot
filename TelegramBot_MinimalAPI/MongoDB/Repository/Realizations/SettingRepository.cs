using MongoDB.Driver;
using TelegramBot_MinimalAPI.MongoDB.Repository.Interfaces;
using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.MongoDB.Repository.Realizations
{
    public class SettingRepository: ISettingRepository
    {
        private readonly IMongoCollection<BaseSetting> _mongoCollection;

        public SettingRepository(IMongoDatabase mongoDatabase)
        {
            _mongoCollection = mongoDatabase.GetCollection<BaseSetting>("Settings");
        }

        public async Task CreateAsync(BaseSetting baseSetting) =>
            await _mongoCollection.InsertOneAsync(baseSetting);

        public async Task DeleteByUserIDAsync(long id) =>
            await _mongoCollection.DeleteOneAsync(e => e.userId == id);


        public async Task<BaseSetting>? GetByUserIDAsync(long id) =>
            await _mongoCollection.Find(e => e.userId == id).FirstOrDefaultAsync();


        public async Task UpdateAsync(BaseSetting baseSetting) =>
            await _mongoCollection.ReplaceOneAsync(e => e.userId == baseSetting.userId, baseSetting);
        
    }
}
