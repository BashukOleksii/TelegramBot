using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.MongoDB.Repository.Interfaces
{
    public interface ISettingRepository
    {
        Task<BaseSetting>? GetByUserIDAsync(long id);
        Task DeleteByUserIDAsync(long id);
        Task CreateAsync(BaseSetting baseSetting);
        Task UpdateAsync(BaseSetting baseSetting);
    }
}
