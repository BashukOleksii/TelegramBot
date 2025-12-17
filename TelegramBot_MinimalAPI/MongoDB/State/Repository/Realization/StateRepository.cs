using MongoDB.Driver;
using System.Net.Http.Headers;
using TelegramBot_MinimalAPI.MongoDB.State.Repository.Interface;

namespace TelegramBot_MinimalAPI.MongoDB.State.Repository.Realization
{
    public class StateRepository: IStateRepository
    {
        private readonly IMongoCollection<CacheUserState> _collection;

        public StateRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<CacheUserState>("UserStates");
        }

        public async Task CreateAsync(CacheUserState cacheUserState) =>
            await _collection.InsertOneAsync(cacheUserState);


        public async Task DeleteAsync(long userId) =>
            await _collection.DeleteManyAsync(s => s.UserId == userId);


        public async Task<CacheUserState?> GetByUserIdAsync(long userId) =>
            await _collection.Find(s => s.UserId == userId).FirstOrDefaultAsync();
        

        public async Task UpdateAsyn(CacheUserState cacheUserState) =>
            await _collection.ReplaceOneAsync(s => s.UserId == cacheUserState.UserId,cacheUserState);
        
    }
}
