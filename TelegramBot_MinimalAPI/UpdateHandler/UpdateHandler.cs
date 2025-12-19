using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot_MinimalAPI.GeocodingAndReverseService;
using TelegramBot_MinimalAPI.MongoDB.Setting.Service.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.State;
using TelegramBot_MinimalAPI.MongoDB.State.Service.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeaterData;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Service.Interface;
using TelegramBot_MinimalAPI.QueryBuilderTool;
using TelegramBot_MinimalAPI.Response;
using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI.UpdateHandler
{

    public class UpdateHandler
    {
        private readonly TelegramBotClient _client;
        private readonly ISettingService _settingService;
        private readonly IStateService _stateService;
        private readonly IWeatherCacheService _weatherCache;
        private readonly IWeatherDataService _weatherDataService;
        private readonly GeocodingServise _geocodingService;
        private float lat = (float)50.45466, lon = (float)30.5238;



        public UpdateHandler
            (TelegramBotClient client,
            ISettingService settingService,
            IStateService stateService,
            IWeatherCacheService weatherCache,
            IWeatherDataService weatherDataService,
            GeocodingServise geocodingServise)
        {
            _client = client;
            _settingService = settingService;
            _stateService = stateService;
            _weatherCache = weatherCache;
            _weatherDataService = weatherDataService;
            _geocodingService = geocodingServise;
        }

        public async Task HandleUpdate(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not null)
                        await HandleMessageUpdate(update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is not null)
                        await HandleCallBackQueryUpdate(update.CallbackQuery);
                    break;


            }
        }

        #region Message
        public async Task HandleMessageUpdate(Message message)
        {

            if (message.Type == MessageType.Location)
            {
                await SetLocationFromMessage(message);
                return;
            }

            if (message.Text is not null)
            {
                if(message.Text.EndsWith("погода")&& !message.Text.StartsWith("Загальна") )
                    await HandleWeatherEnableSettingButton(message);

                switch (message.Text!.ToLower())
                {
                    case "/start":
                        await HandleCommandStart(message);
                        break;

                    case "використовувати за замовчуванням":
                        await SetDefaultLocation(message);
                        break;
                    case "встановити вручну":
                        {
                            await _client.SendMessage(message.Chat.Id, "Введіть назву");
                            await _stateService.SetState(message.From!.Id, UserStates.WrittingLocationName);
                        }
                        break;



                    case "налаштування":
                        await HandleSettingButton(message.Chat.Id);
                        break;
                    case "<-- назад":
                        await SetMainButtons(message.Chat.Id);
                        break;
                    case "користувацькі налаштування":
                        await HandleUserSettingButton(message);
                        break;
                    case "погода":
                        await HandleWeatherButton(message);
                        break;



                    default:
                        await SetLocationFromName(message);
                        break;
                }

            }

        }

        #region Початкові дії
        private async Task HandleCommandStart(Message message)
        {
            if (!await _settingService.IsExist(message.From.Id))
            {
                var keyBoard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Використовувати за замовчуванням"),
                        new KeyboardButton("Встановити вручну")
                    },
                    new KeyboardButton[]
                    {
                        KeyboardButton.WithRequestLocation("Надіслати своє положення")
                    }
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };


                await _client.SendMessage(message.Chat.Id, "Привіт, виберіть налаштування", replyMarkup: keyBoard);
                await _client.SendMessage(
                        message.Chat.Id,
                        "ПОПЕРЕДЖЕННЯ функція \"Надіслати своє положення\" потребує дозволу в налаштуваннях" +
                        "\nЯкщо не прийшла зворотня відповідь спробуйте змінити дозволи в налаштуванні додатку " +
                        "\nАбо встановіть за допомогою назви місця" +
                        "\n/start - для того, щоб знову почати"
                );
            }
            else
                await SetMainButtons(message.Chat.Id);
        }
       
        private async Task SetMainButtons(long chatID)
        {
            var keyBoard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Погода")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Налаштування")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Інформація")
                }
            })
            {
                ResizeKeyboard = true
            };

            await _client.SendMessage(chatID, "Вибери дію", replyMarkup: keyBoard);
        }
        private async Task SetDefaultLocation(Message message)
        {
            var first = !await _settingService.IsExist(message.From!.Id);
            var setting = await _settingService.GetSettingAsync(message.From!.Id, lat, lon);
            if (!first)
            {
                setting.Latitude = lat;
                setting.Longtitude = lon;
                await _settingService.Update(setting);
            }
            else
                await SetMainButtons(message.Chat.Id);


            await _client.SendMessage(message.Chat.Id, "Встановлено відстеження погоди для міста Київ");

            if(!first)
                await HandleSettingButton(message.Chat.Id);


        }
        private async Task SetLocationFromMessage(Message message)
        {

            float latitude = (float)message.Location.Latitude;
            float longtitude = (float)message.Location.Longitude;

            bool first = !await _settingService.IsExist(message.From!.Id);
            var userSetting = await _settingService.GetSettingAsync(message.From.Id, latitude, longtitude);
            if (!first)
            {
                userSetting.Latitude = latitude;
                userSetting.Longtitude = longtitude;
                await _settingService.Update(userSetting);
            }
            else
                await SetMainButtons(message.Chat.Id);

            var locationName = await _geocodingService.GetNameAsync(userSetting.Latitude, userSetting.Longtitude);

            await _client.SendMessage(message.Chat.Id, $"Встановлено місце відстеження: {locationName}");

            if(!first)
                await HandleSettingButton(message.Chat.Id);


        }
        private async Task SetLocationFromName(Message message)
        {
            var userState = await _stateService.GetStateAsync(message.From.Id);

            if (userState != UserStates.WrittingLocationName || message.Text is null)
                return;

            var location = await _geocodingService.GetPointAsync(message.Text);
            if (location is null)
            {
                await _client.SendMessage(message.Chat.Id, "Проблема із назвою, не вийшло знайти, введіть ще раз");
                return;
            }

            bool firstSetting = !await _settingService.IsExist(message.From.Id);
            var userSetting = await _settingService.GetSettingAsync(message.Chat.Id, location.Latitude, location.Longitude);
            if (!firstSetting)
            {
                userSetting.Latitude = location.Latitude;
                userSetting.Longtitude = location.Longitude;
                await _settingService.Update(userSetting);
            }
            else
                await SetMainButtons(message.Chat.Id);


            var locationName = await _geocodingService.GetNameAsync(userSetting.Latitude, userSetting.Longtitude);

            await _client.SendMessage(message.Chat.Id, $"Встановлено місце відстеження {locationName}");
            await _stateService.SetState(message.From.Id, UserStates.None);

            if(!firstSetting)
                await HandleSettingButton(message.Chat.Id);
        }

        #endregion







        #region Налаштування
        private async Task HandleSettingButton(long chatID)
        {
            var keyBoard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Користувацькі налаштування")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Поточна погода"),
                    new KeyboardButton("Погодинна погода"),
                    new KeyboardButton("Поденна погода")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("<-- Назад")
                }

            })
            {
                ResizeKeyboard = true
            };

            await _client.SendMessage(chatID, "Виберіть, що саме налаштувати", replyMarkup: keyBoard);
        }
        private async Task HandleUserSettingButton(Message message)
        {
            var setting = await _settingService.GetSettingAsync(message.From.Id, lat, lon);

            if (setting is null)
            {
                await _client.SendMessage(message.Chat.Id, "Помилка отримання інформації");
                return;
            }

            var city = await _geocodingService.GetNameAsync(setting.Latitude, setting.Longtitude);


            var stringAnswer = "Користовацькі налаштування:" +
                "\nМісце відстеження: " + city +
                "\nКількість відстеження майбутніх днів: " + (setting.ForecastDays ?? 7) +
                "\nКількість відстеження минулих днів: " + (setting.PastDays ?? 0) +
                "\nОдиниця вимірювання температури: " + (setting.TempUnit ?? "°C") +
                "\nОдиниця вимірювання швидкості: " + (setting.WindSpeed ?? "kh/h") +
                "\nБажаєте щось змінити?";

            var buttons = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Місце відстеження", callbackData: "place")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Кількість майбутніх днів", callbackData : "days"),
                    InlineKeyboardButton.WithCallbackData(text: "Кількість минулих днів", callbackData : "pastDays")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text : "Одниці виміру температури", callbackData: "temp"),
                    InlineKeyboardButton.WithCallbackData(text: "Одиниці виміру швидкості", callbackData : "speed")
                }
            });

            await _client.SendMessage(message.Chat.Id, stringAnswer, replyMarkup: buttons);
        }
        private async Task HandleWeatherEnableSettingButton(Message message)
        {
            var setting = await _settingService.GetSettingAsync(message.From!.Id);

            string type = message.Text.Split(' ')[0];

            InlineKeyboardMarkup keyBoard = type switch
            {
                "Поточна" => GetCurentSettingKeyBoad(setting.CurentSetting),
                "Погодинна" => GetHourlySettingKeyBoad(setting.HourlySetting),
                "Поденна" => GetDailySettingKeyBoad(setting.DailySetting)
            };

            await _client.SendMessage(
                message.Chat.Id,
                "Налаштування відображення поточної погоди:",
                replyMarkup: keyBoard
                );
        }
        

        #endregion





        #region Допоміжні
        private string GetSymb(bool enable) => enable ? "✔️" : "❌";
        private InlineKeyboardMarkup GetCurentSettingKeyBoad(CurentWeatherSetting curentSetting)
        {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Поточна температура " + GetSymb(curentSetting.Temperature),callbackData:"c:Temperature")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Відносна вологість " + GetSymb(curentSetting.RelativeHumidity),callbackData:"c:RelativeHumidity")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Поточна температура (відчувається як) " + GetSymb(curentSetting.ApperentTemperature),callbackData:"c:ApperentTemperature")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Швидксть вітру" + GetSymb(curentSetting.WindSpeed),callbackData:"c:WindSpeed")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Напрмок вітру " + GetSymb(curentSetting.WindDirection),callbackData:"c:WindDirection")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Загальна кількість опадів " + GetSymb(curentSetting.Precipitation),callbackData:"c:Precipitation")
                },
                 new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Кількість дощу " + GetSymb(curentSetting.Rain),callbackData:"c:Rain")
                },
                 new InlineKeyboardButton[]
                 {
                     InlineKeyboardButton.WithCallbackData("Кількіть снігопаду " + GetSymb(curentSetting.SnowFall),callbackData:"c:SnowFall")
                 },
                 new InlineKeyboardButton[]
                 {
                     InlineKeyboardButton.WithCallbackData("Поверхневий тиск " + GetSymb(curentSetting.SurfacePressure),callbackData:"c:SurfacePressure")
                 },
                 new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Відсоток хмар на небі " + GetSymb(curentSetting.CloudCover),callbackData:"c:CloudCover")
                }

            });
        }
        private InlineKeyboardMarkup GetHourlySettingKeyBoad(HourlyWeatherSetting hourlySetting)
        {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Температура " + GetSymb(hourlySetting.Temperature),callbackData:"h:Temperature")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Відносна вологість " + GetSymb(hourlySetting.RelativeHumidity),callbackData:"h:RelativeHumidity")
                },

                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Ймовірність дощу " + GetSymb(hourlySetting.PrecipitationProbality),callbackData:"h:PrecipitationProbality")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Видимість " + GetSymb(hourlySetting.Visibility),callbackData:"h:Visibility")
                },


                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Точка роси " + GetSymb(hourlySetting.DewPoint),callbackData:"h:DewPoint")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Швидість вітру " + GetSymb(hourlySetting.WindSpeed),callbackData:"h:WindSpeed")
                },
                 new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Пориви вітру " + GetSymb(hourlySetting.WindGusts),callbackData:"h:WindGusts")
                },
                 new InlineKeyboardButton[]
                 {
                     InlineKeyboardButton.WithCallbackData("Напрямок вітру " + GetSymb(hourlySetting.WindDirection),callbackData:"h:WindDirection")
                 },
                 new InlineKeyboardButton[]
                 {
                     InlineKeyboardButton.WithCallbackData("Відсоток хмар на небі " + GetSymb(hourlySetting.CloudCover),callbackData:"h:CloudCover")
                 }

            });


        }
        private InlineKeyboardMarkup GetDailySettingKeyBoad(DailyWeatherSetting dailySetting) 
        {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Максимальна температура " + GetSymb(dailySetting.MaxTemp),callbackData:"d:MaxTemp")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Мінімальна температура " + GetSymb(dailySetting.MinTemp),callbackData:"d:MinTemp")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Як відчувається максимальна температура" + GetSymb(dailySetting.ApperentMax),callbackData:"d:ApperentMax")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Як відчувається мінімальна температура" + GetSymb(dailySetting.ApperentMin),callbackData:"d:ApperentMin")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Максимальна швидкість вітру" + GetSymb(dailySetting.WindSpeedMax),callbackData:"d:WindSpeedMax")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Переважний напрямок вітру" + GetSymb(dailySetting.DominantWindDirection),callbackData:"d:DominantWindDirection")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Схід сонця" + GetSymb(dailySetting.SunRise),callbackData:"d:SunRise")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Захід сонця" + GetSymb(dailySetting.SunSet),callbackData:"d:SunSet")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Тривалість світловго дня" + GetSymb(dailySetting.DayLightDuration),callbackData:"d:DayLightDuration")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Загальна кількість опадів" + GetSymb(dailySetting.PrecipitationSum),callbackData:"d:PrecipitationSum")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Кількість опадів від дощу" + GetSymb(dailySetting.RainSum),callbackData:"d:RainSum")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Кількість опадів від зливи" + GetSymb(dailySetting.ShowersSum),callbackData:"d:ShowersSum")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Кількість опадів від снігу" + GetSymb(dailySetting.SnowdallSum),callbackData:"d:SnowdallSum")
                },
            });
        }
        #endregion




        #region Погода

        private async Task HandleWeatherButton(Message message)
        {
            await _client.SendChatAction(message.Chat.Id, ChatAction.Typing);

            var queryBuilder = new QueryBuilder();

            var userSetting = await _settingService.GetSettingAsync(message.From!.Id);

            queryBuilder
                .SetSetting(userSetting!)
                .AddType<CurentWeatherSetting>()
                .AddType<HourlyWeatherSetting>()
                .AddType<DailyWeatherSetting>();

            bool needToRequest;

            var responseCache = await _weatherCache.Get(message.From!.Id);

            string query = queryBuilder.Build();
            BaseResponse response;

            try
            {
                if (responseCache is null || responseCache.Key != query)
                {
                    using var _httpClient = new HttpClient();
                    response = await _httpClient.GetFromJsonAsync<BaseResponse>(query);

                    var cacheToSave = new WeatherCache()
                    {
                        UserId = message.From.Id,
                        Key = query,
                        Cache = response,
                        ExpiredTime = DateTime.UtcNow.AddHours(1)
                    };

                    await _weatherCache.Send(cacheToSave);
                }
                else
                    response = responseCache.Cache;
            }
            catch 
            {
                await _client.SendMessage(
                    message.Chat!.Id,
                    "Відбулась помилка під час запиту, спробуйте пізніше");

                return;

            }

            var data = response!.GetInfo(userSetting!.TempUnit == null ? "°C" : "°F", userSetting.WindSpeed == null ? "км/год" : "м/с");

            WeatherDataEntity weatherDataEntity = new WeatherDataEntity();

            if (data["current"] is not null)
                await _client.SendMessage(
                    message.Chat.Id,
                    data["current"]!
                    );
            if (data["daily"] is not null)
            {
                var dailyData = data["daily"]!.Split("_____");

                weatherDataEntity.DailyArray = dailyData.ToList();
                weatherDataEntity.DailyIndex = 0;

                await _client.SendMessage(
                    message.Chat.Id,
                    dailyData[0],
                    replyMarkup: SetMoveButtons(WeatherPageType.Daily)
                    );

            }
            if (data["hourly"] is not null)
            {
                var hourlyData = data["hourly"]!.Split("_____");
                
                weatherDataEntity.HourlyArray = hourlyData.ToList();
                weatherDataEntity.HourlyIndex = 0;

                await _client.SendMessage(
                    message.Chat.Id,
                    hourlyData[0],
                    replyMarkup: SetMoveButtons(WeatherPageType.Hourly)
                    );

            }

            if(weatherDataEntity.DailyArray is not null ||
                weatherDataEntity.HourlyArray is not null)
            {
                weatherDataEntity.UserId = message.From.Id;
                await _weatherDataService.SetHourlyData(weatherDataEntity);
            }

        }
       

        private InlineKeyboardMarkup SetMoveButtons(WeatherPageType weatherPageType)
        {
            return new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("⏴",callbackData:$"move_back_{weatherPageType}"),
                InlineKeyboardButton.WithCallbackData("⏵",callbackData:$"move_forvard{weatherPageType}")
            });
        }
        #endregion





        #endregion

        #region CallBackQuery
        private async Task HandleCallBackQueryUpdate(CallbackQuery callbackQuery)
        {
            var data = callbackQuery.Data;

            if (string.IsNullOrEmpty(data))
                return;

            if (data.StartsWith("c:") || data.StartsWith("h:") || data.StartsWith("d:"))
                await HandleEnableSettingCallBackQuery(callbackQuery);
            
            if (data.Contains("_days"))
                await SetDaysUserSetting(callbackQuery);

            if (data.StartsWith("temp_"))
                await SetTempUnits(callbackQuery);

            if (data.StartsWith("speed_"))
                await SetSpeedUnits(callbackQuery);     
            
            switch (data)
            {

                case "place":
                    await HandleCommandStart(callbackQuery.Message!);
                    break;

                case "days":
                    await HandleDaysCallBack(callbackQuery.Message!.Chat.Id);
                    break;

                case "pastDays":
                    await HandlePastDaysCallBack(callbackQuery.Message!.Chat.Id);
                    break;
              
                case "temp":
                    await HandleTempCallBack(callbackQuery.Message!.Chat.Id);
                    break;

                case "speed":
                    await HandleSpeedCallBack(callbackQuery.Message!.Chat.Id);
                    break;
            }


            await _client.AnswerCallbackQuery(callbackQuery.Id);


        }

        #region Personal

        #region Days
        private async Task HandleDaysCallBack(long chatID)
        {
            //1 3 7 9 14 15
            var keyBoard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("1 день", callbackData: "1_days"),
                InlineKeyboardButton.WithCallbackData("3 дні", callbackData: "3_days"),
                InlineKeyboardButton.WithCallbackData("7 днів", callbackData: "7_days"),
                InlineKeyboardButton.WithCallbackData("9 днів", callbackData: "9_days"),
                InlineKeyboardButton.WithCallbackData("14 днів", callbackData: "14_days"),
                InlineKeyboardButton.WithCallbackData("15 днів", callbackData: "15_days"),
            });

            await _client.SendMessage(
                chatID,
                "Виберіть період інформації про майбутню погоду",
                replyMarkup: keyBoard
                );

        }
        private async Task HandlePastDaysCallBack(long chatID)
        {
            // 0 1 2 3 5 7 14 31 61
            var keyBoard = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Не потрібно", callbackData: "0_days_past"),
                    InlineKeyboardButton.WithCallbackData("2 дні", callbackData: "2_days_past"),
                    InlineKeyboardButton.WithCallbackData("3 дні", callbackData: "3_days_past"),
                    InlineKeyboardButton.WithCallbackData("5 днів", callbackData: "5_days_past")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("7 днів", callbackData: "7_days_past"),
                    InlineKeyboardButton.WithCallbackData("14 днів", callbackData: "14_days_past"),
                    InlineKeyboardButton.WithCallbackData("31 днів", callbackData: "31_days_past"),
                    InlineKeyboardButton.WithCallbackData("61 днів", callbackData: "61_days_past")
                }
            });

            await _client.SendMessage(
                chatID,
                "Виберіть період інформації про минулу погоду погоду",
                replyMarkup: keyBoard
                );
        }
        private async Task SetDaysUserSetting(CallbackQuery callbackQuery)
        {
            var parts = callbackQuery.Data!.Split('_');
            int days = int.Parse(parts[0]);
            var setting = await _settingService.GetSettingAsync(callbackQuery!.From.Id, lat, lon);

            if (parts.Length == 2)
                setting!.ForecastDays = days == 7 ? null : days;
            else
                setting!.PastDays = days == 0 ? null : days;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callbackQuery.Message!.Chat.Id,
                "Встановлено значення для кількості днів: " + days); ;
        }

        #endregion


        #region Temp
        private async Task HandleTempCallBack(long chatID)
        {
            // farenheit
            var keyBoard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {

                InlineKeyboardButton.WithCallbackData("Цельсії", callbackData: "temp_celcium"),
                InlineKeyboardButton.WithCallbackData("Фаренгейти", callbackData: "temp_fahrenheit")

            });

            await _client.SendMessage(
                chatID,
                "Виберіть в яких одиницях відображати",
                replyMarkup: keyBoard
                );
        }

        private async Task SetTempUnits(CallbackQuery callBackQuery)
        {
            var units = callBackQuery.Data.Substring(5);

            var setting = await _settingService.GetSettingAsync(callBackQuery.From!.Id);

            setting!.TempUnit = units == "celcium" ? null : units;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callBackQuery.Message!.Chat.Id,
                "Встановлено одиниці вимірювання: градуси " + (units == "celcium" ? "цельсія" : "фаренгейта")
                );

        }

        #endregion


        #region Speed
        private async Task HandleSpeedCallBack(long chatID)
        {
            // ms
            var keyBoard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {

                InlineKeyboardButton.WithCallbackData("гм/год", callbackData: "speed_kh"),
                InlineKeyboardButton.WithCallbackData("м/с", callbackData: "speed_ms")

            });

            await _client.SendMessage(
                chatID,
                "Виберіть в яких одиницях відображати",
                replyMarkup: keyBoard
                );
        }
        private async Task SetSpeedUnits(CallbackQuery callBackQuery)
        {
            var units = callBackQuery.Data!.Substring(6);

            var setting = await _settingService.GetSettingAsync(callBackQuery.From!.Id);

            setting!.WindSpeed = units == "kh" ? null : units;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callBackQuery.Message!.Chat.Id,
                "Встановлено одиниці вимірювання: " + (units == "kh" ? "км/год" : "м/c")
                );

        }
        #endregion

        #endregion 

        private async Task HandleEnableSettingCallBackQuery(CallbackQuery callbackQuery)
        {
            string prefix = callbackQuery.Data.Substring(0,2);
            string propertyName = callbackQuery.Data.Substring(2);

            var setting = await _settingService.GetSettingAsync(callbackQuery.From!.Id);

            object settingObject = prefix switch
            {
                "c:" => setting.CurentSetting,
                "h:" => setting.HourlySetting,
                "d:" => setting.DailySetting
            };

            
            PropertyInfo propertyInfo = settingObject.GetType().GetProperty(propertyName);
            bool enable = !(bool)propertyInfo.GetValue(settingObject);
            propertyInfo.SetValue(settingObject, enable);

            await _settingService.Update(setting);

            InlineKeyboardMarkup keyBoard = prefix switch {
                "c:" => GetCurentSettingKeyBoad(setting.CurentSetting),
                "h:" => GetHourlySettingKeyBoad(setting.HourlySetting),
               
                "d:" => GetDailySettingKeyBoad(setting.DailySetting)
            };

            await _client.EditMessageReplyMarkup(
                callbackQuery.Message.Chat.Id,
                 callbackQuery.Message.Id,
                 replyMarkup: keyBoard
                );

        }





        #endregion


    }
}
