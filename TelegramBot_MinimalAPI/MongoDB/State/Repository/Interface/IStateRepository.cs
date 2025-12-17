namespace TelegramBot_MinimalAPI.MongoDB.State.Repository.Interface
{
    public interface IStateRepository
    {
        Task CreateAsync(CacheUserState cacheUserState);
        Task<CacheUserState?> GetByUserIdAsync(long userId);
        Task DeleteAsync(long userId);
        Task UpdateAsyn(CacheUserState cacheUserState);
    }
}
