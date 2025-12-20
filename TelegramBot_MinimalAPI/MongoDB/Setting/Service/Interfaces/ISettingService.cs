using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.MongoDB.Setting.Service.Interfaces
{
    public interface ISettingService
    {
        Task<BaseSetting?> GetSettingAsync(long userId, float lat = (float)50.45466, float lon = (float)30.5238);
        Task<bool> Update(BaseSetting baseSetting);
        Task<bool> Delete(long userId);

        Task<bool> IsExist(long userId);
    }
}
