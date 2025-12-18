using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot_MinimalAPI.GeocodingAndReverseService;
using TelegramBot_MinimalAPI.MongoDB.Setting.Service.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.State;
using TelegramBot_MinimalAPI.MongoDB.State.Service.Interface;
using TelegramBot_MinimalAPI.Setting;

namespace TelegramBot_MinimalAPI
{

    public class UpdateHandler
    {
        private readonly TelegramBotClient _client;
        private readonly ISettingService _settingService;
        private readonly IStateService _stateService;
        private readonly GeocodingServise _geocodingService;
        private float lat = (float)50.45466, lon = (float)30.5238;


        public UpdateHandler
            (TelegramBotClient client,
            ISettingService settingService,
            IStateService stateService,
            GeocodingServise geocodingServise)
        {
            _client = client;
            _settingService = settingService;
            _geocodingService = geocodingServise;
            _stateService = stateService;
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
                    case "поточна погода":
                        await HandleCurentSettingButton(message);
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
                    new KeyboardButton("Загальна + поточна погода"),
                    new KeyboardButton("Погодинна погода"),
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
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await _client.SendMessage(chatID, "Вибери дію", replyMarkup: keyBoard);
        }
        #region SetLocation
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
            await _client.SendMessage(message.Chat.Id, "Встановлено відстеження погоди для міста Київ");
            await SetMainButtons(message.Chat.Id);
        }
        private async Task SetLocationFromMessage(Message message)
        {

            float latitude = (float)message.Location.Latitude;
            float longtitude = (float)message.Location.Longitude;

            bool first = !(await _settingService.IsExist(message.From!.Id));
            var userSetting = await _settingService.GetSettingAsync(message.From.Id, latitude, longtitude);
            if (!first)
            {
                userSetting.Latitude = latitude;
                userSetting.Longtitude = longtitude;
                await _settingService.Update(userSetting);
            }

            var locationName = await _geocodingService.GetNameAsync(userSetting.Latitude, userSetting.Longtitude);

            await _client.SendMessage(message.Chat.Id, $"Встановлено місце відстеження: {locationName}");

            await SetMainButtons(message.Chat.Id);
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

            bool firstSetting = !(await _settingService.IsExist(message.From.Id));
            var userSetting = await _settingService.GetSettingAsync(message.Chat.Id, location.Latitude, location.Longitude);
            if (!firstSetting)
            {
                userSetting.Latitude = location.Latitude;
                userSetting.Longtitude = location.Longitude;
                await _settingService.Update(userSetting);
            }


            var locationName = await _geocodingService.GetNameAsync(userSetting.Latitude, userSetting.Longtitude);

            await _client.SendMessage(message.Chat.Id, $"Встановлено місце відстеження {locationName}");
            await SetMainButtons(message.Chat.Id);
            await _stateService.SetState(message.From.Id, UserStates.None);
        }
        #endregion

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
                ResizeKeyboard = true,
                OneTimeKeyboard = true
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
        private async Task HandleCurentSettingButton(Message message)
        {
            var setting = await _settingService.GetSettingAsync(message.From!.Id);

            var curentSetting = setting!.CurentSetting;

            await _client.SendMessage(
                message.Chat.Id,
                "Налаштування відображення поточної погоди:",
                replyMarkup: GetCurentSettingKeyBoad(curentSetting)
                );
        }

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
        #endregion

        #endregion

        #region CallBackQuery
        private async Task HandleCallBackQueryUpdate(CallbackQuery callbackQuery)
        {
            var data = callbackQuery.Data;

            if (string.IsNullOrEmpty(data))
                return;

            switch (data)
            {
                #region UserSetting
                case "place":
                    await HandleCommandStart(callbackQuery.Message!);
                    break;

                case "days":
                    await HandleDaysCallBack(callbackQuery.Message!.Chat.Id);
                    break;

                case "1_days":
                case "3_days":
                case "7_days":
                case "9_days":
                case "14_days":
                case "16_days":
                    await SetForecastDays(callbackQuery);
                    break;

                case "pastDays":
                    await HandlePastDaysCallBack(callbackQuery.Message!.Chat.Id);
                    break;

                case "0_days_past":
                case "2_days_past":
                case "3_days_past":
                case "5_days_past":
                case "7_days_past":
                case "14_days_past":
                case "31_days_past":
                case "61_days_past":
                    await SetPastDays(callbackQuery);
                    break;


                case "temp":
                    await HandleTempCallBack(callbackQuery.Message!.Chat.Id);
                    break;

                case "celcium":
                case "fahrenheit":
                    await SetTempUnits(callbackQuery);
                    break;



                case "speed":
                    await HandleSpeedCallBack(callbackQuery.Message!.Chat.Id);
                    break;

                case "kh":
                case "ms":
                    await SetSpeedUnits(callbackQuery);
                    break;

                    #endregion
            }

            if (data.StartsWith("c:"))
                await HandleCurerentSettingCallBackQuery(callbackQuery);

            await _client.AnswerCallbackQuery(callbackQuery.Id);


        }

        #region Personal
        #region Days
        private async Task HandleDaysCallBack(long chatID)
        {
            //1 3 7 9 14 16
            var keyBoard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("1 день", callbackData: "1_days"),
                InlineKeyboardButton.WithCallbackData("3 дні", callbackData: "3_days"),
                InlineKeyboardButton.WithCallbackData("7 днів", callbackData: "7_days"),
                InlineKeyboardButton.WithCallbackData("9 днів", callbackData: "9_days"),
                InlineKeyboardButton.WithCallbackData("14 днів", callbackData: "14_days"),
                InlineKeyboardButton.WithCallbackData("16 днів", callbackData: "16_days"),
            });

            await _client.SendMessage(
                chatID,
                "Виберіть період інформації про майбутню погоду",
                replyMarkup: keyBoard
                );

        }

        private async Task SetForecastDays(CallbackQuery callbackQuery)
        {
            string countDays = callbackQuery.Data!;
            int days = int.Parse(countDays.Split('_')[0]);

            var setting = await _settingService.GetSettingAsync(callbackQuery!.From.Id, lat, lon);

            setting!.ForecastDays = days == 7 ? null : days;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callbackQuery.Message!.Chat.Id,
                "Встановлено значення для кількості днів показу: " + days); ;
        }
        #endregion
        #region PastDays
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

        private async Task SetPastDays(CallbackQuery callBackQuery)
        {
            string countDays = callBackQuery.Data!;
            int days = int.Parse(countDays.Split('_')[0]);

            var setting = await _settingService.GetSettingAsync(callBackQuery.From!.Id);

            setting!.PastDays = days == 0 ? null : days;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callBackQuery.Message!.Chat.Id,
                "Встановлено кількість відстеження минулих днів: " + days
                );

        }
        #endregion
        #region Temp
        private async Task HandleTempCallBack(long chatID)
        {
            // farenheit
            var keyBoard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {

                InlineKeyboardButton.WithCallbackData("Цельсії", callbackData: "celcium"),
                InlineKeyboardButton.WithCallbackData("Фаренгейти", callbackData: "fahrenheit")

            });

            await _client.SendMessage(
                chatID,
                "Виберіть в яких одиницях відображати",
                replyMarkup: keyBoard
                );
        }

        private async Task SetTempUnits(CallbackQuery callBackQuery)
        {

            var setting = await _settingService.GetSettingAsync(callBackQuery.From!.Id);

            setting!.TempUnit = callBackQuery.Data == "celcium" ? null : callBackQuery.Data!;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callBackQuery.Message!.Chat.Id,
                "Встановлено одиниці вимірювання: градуси " + (callBackQuery.Data == "celcium" ? "цельсія" : "фаренгейта")
                );

        }
        #endregion
        #region Speed
        private async Task HandleSpeedCallBack(long chatID)
        {
            // ms
            var keyBoard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {

                InlineKeyboardButton.WithCallbackData("гм/год", callbackData: "kh"),
                InlineKeyboardButton.WithCallbackData("м/с", callbackData: "ms")

            });

            await _client.SendMessage(
                chatID,
                "Виберіть в яких одиницях відображати",
                replyMarkup: keyBoard
                );
        }
        private async Task SetSpeedUnits(CallbackQuery callBackQuery)
        {

            var setting = await _settingService.GetSettingAsync(callBackQuery.From!.Id);

            setting!.TempUnit = callBackQuery.Data == "kh" ? null : callBackQuery.Data!;

            await _settingService.Update(setting);

            await _client.SendMessage(
                callBackQuery.Message!.Chat.Id,
                "Встановлено одиниці вимірювання: " + (callBackQuery.Data == "kh" ? "км/год" : "м/c")
                );

        }
        #endregion
        #endregion
        #region
        private async Task HandleCurerentSettingCallBackQuery(CallbackQuery callbackQuery)
        {
            var setting = await _settingService.GetSettingAsync(callbackQuery.From!.Id);

            string propertyName = callbackQuery.Data!.Substring(2);
            SetPropetyValue(setting.CurentSetting, propertyName);
            await _settingService.Update(setting);

            await _client.EditMessageReplyMarkup(
                callbackQuery.Message.Chat.Id,
                 callbackQuery.Message.Id,
                 replyMarkup: GetCurentSettingKeyBoad(setting.CurentSetting)
                );

        }

        private async Task SetPropetyValue(object setting, string propertyName)
        {
            PropertyInfo propertyInfo = setting.GetType().GetProperty(propertyName);
            bool enable = !(bool)propertyInfo.GetValue(setting);
            propertyInfo.SetValue(setting, enable);
        }
        #endregion




        #endregion


    }
}
