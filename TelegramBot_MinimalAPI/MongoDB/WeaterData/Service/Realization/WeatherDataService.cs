using TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Interface;

namespace TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Realization
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly IWeatherDataRepository _weatherDataRepository;

        public WeatherDataService(IWeatherDataRepository weatherDataRepository)
        {
            _weatherDataRepository = weatherDataRepository;
        }

        public async Task<WeatherDataEntity?> GetData(long userID) =>
            await _weatherDataRepository.GetAsync(userID);



        public async Task<string?> GetPage(long userId, string propertyName, int index)
        {
            var weatherData = await _weatherDataRepository.GetAsync(userId);

            if (weatherData is null)
                return null;

            try
            {


                var propertyInfo = weatherData.GetType().GetProperty(propertyName);

                var pages = (List<string>)propertyInfo.GetValue(weatherData);


                string page;

                if (propertyInfo.Name == "hourlyArray")
                {
                    page = pages[index];
                    weatherData.HourlyIndex = index;
                }
                else
                {
                    page = pages[index];
                    weatherData.DailyIndex = index;
                }


                await _weatherDataRepository.UpdateAsync(weatherData);

                return page;
            }
            catch
            {
                return null;
            }
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
