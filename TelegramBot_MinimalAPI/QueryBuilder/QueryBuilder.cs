using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Reflection;
using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.QueryBuilder
{
    public class QueryBuilder
    {
        private BaseSetting _userSetting;
        public const string baseUrl = "https://api.open-meteo.com/v1/forecast?";
        string url = "";

        public QueryBuilder SetSetting(BaseSetting baseSetting)
        {
            _userSetting = baseSetting;

            var queries = new Dictionary<string, string>();
            var properties = typeof(BaseSetting).GetProperties();

            foreach (var property in properties)
            {
                if (IsClass(property.PropertyType))
                    continue;

                var value = property.GetValue(_userSetting);

                if(value is null)
                    continue;

                var key = property.GetCustomAttribute<BsonElementAttribute>()!.ElementName;

                queries[key] = value.ToString();
            }

            url = baseUrl + string.Join('&', queries.Select(d => $"{d.Key}={d.Value}"));

            return this;
        }

        public QueryBuilder AddType<T>()
        {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url пустий, потрібно встановити налашування");

            if (typeof(T) == typeof(CurentWeatherSetting))
                url += "&current=";
            else if (typeof(T) == typeof(HourlyWeatherSetting))
                url += "&hourly=";
            else if (typeof(T) == typeof(DailyWeatherSetting))
                url += "&daily=";
            else
                throw new ArgumentException("Не правильно передане значення");


            var settingBase = typeof(BaseSetting).
                GetProperties().FirstOrDefault(p => p.PropertyType == typeof(T));

            var needSetting = settingBase.GetValue(_userSetting);

            var paramentrs = needSetting.GetType().GetProperties().Where(p => (bool)p.GetValue(needSetting)! == true);

            url += string.Join(',', paramentrs.Select(p => p.GetCustomAttribute<BsonElementAttribute>()!.ElementName));
    
            return this;
        }

        public string? Build()
        {
            if (string.IsNullOrEmpty(url))
                return null;

            return url;
        } 
        private bool IsClass(Type t)
        {
            return t == typeof(CurentWeatherSetting) || 
                t == typeof(HourlyWeatherSetting) ||
                t == typeof(DailyWeatherSetting);
        }
    }
}
