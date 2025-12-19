using TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Interface;

namespace TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Realization
{
    public class WeatherDataService: IWeatherDataService
    {
        private readonly IWeatherDataRepository _weatherDataRepository;

        public WeatherDataService(IWeatherDataRepository weatherDataRepository)
        {
            _weatherDataRepository = weatherDataRepository;
        }

        public async Task<WeatherDataEntity?> GetData(int userID) =>
            await _weatherDataRepository.GetAsync(userID);
            
        

        public async Task<string?> GetDate(int userIndex, int rowIndex, string propName)
        {
            var data = await _weatherDataRepository.GetAsync(userIndex);
            if (data is null)
                return null;

            var propertyInfo = data.GetType().GetProperty(propName);

            if (propertyInfo is null) 
                return null;

            var array = (List<string>)propertyInfo.GetValue(data);

            if(array is null || array.Count <= rowIndex || rowIndex < 0)
                return null;

            return array[rowIndex];
        }

        public async Task SetHourlyData(WeatherDataEntity weatherDataEntity)
        {
            var data = await _weatherDataRepository.GetAsync(weatherDataEntity.UserId);

            if (data is null)
                await _weatherDataRepository.UpdateAsync(weatherDataEntity);
            else
                await _weatherDataRepository.UpdateAsync(weatherDataEntity);
        }
    }
}
