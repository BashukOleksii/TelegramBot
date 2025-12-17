using TelegramBot_MinimalAPI.MongoDB.Setting.Repository.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.Setting.Service.Interfaces;
using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.MongoDB.Setting.Service.Realizations
{
    public class SettingService: ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        public SettingService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<bool> Delete(long userId)
        {
            try
            {
                if (await IsExist(userId))
                    await _settingRepository.DeleteByUserIDAsync(userId);

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<BaseSetting?> GetSettingAsync(long userId,float lat = (float)50.45466, float lon = (float)30.5238)
        {
            try
            {
                var setting = await _settingRepository.GetByUserIDAsync(userId);

                if (setting is null)
                {
                    var baseSetting = new BaseSetting() { userId = userId, Longtitude = lon, Latitude = lat };
                    await _settingRepository.CreateAsync(baseSetting);
                    return baseSetting;
                }

                return setting;

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Помилка отримання: {ex.Message}, userId: {userId}");
                return null;
            }
        }

        public async Task<bool> Update(BaseSetting baseSetting)
        {
            try
            {
                var setting = await _settingRepository.GetByUserIDAsync(baseSetting.userId);

                if (setting is null)
                    return false;

                baseSetting._id = setting._id;

                await _settingRepository.UpdateAsync(baseSetting);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsExist(long userId)
        {
            var setting = await _settingRepository.GetByUserIDAsync(userId);
            return setting != null;
        }

        
    }
}
