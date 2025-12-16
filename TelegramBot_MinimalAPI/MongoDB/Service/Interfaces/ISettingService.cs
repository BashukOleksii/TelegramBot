using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.MongoDB.Service.Interfaces
{
    public interface ISettingService
    {
        Task<BaseSetting>? GetSettingAsync(long userId, float lat, float lon);
        Task<bool> Update(BaseSetting baseSetting);
        Task<bool> Delete(long userId);
    }
}
