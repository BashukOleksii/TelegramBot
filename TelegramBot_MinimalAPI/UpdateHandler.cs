using System.Reflection.Metadata;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot_MinimalAPI.GeocodingAndReverseService;
using TelegramBot_MinimalAPI.MongoDB.Setting.Service.Interfaces;

namespace TelegramBot_MinimalAPI
{

    public class UpdateHandler
    {
        private readonly TelegramBotClient _client;
        private readonly ISettingService _settingService;
        private readonly GeocodingServise _geocodingService;
        private float lat = (float)50.45466, lon = (float)30.5238;


        public UpdateHandler(TelegramBotClient client, ISettingService settingService, GeocodingServise geocodingServise)
        {
            _client = client;
            _settingService = settingService;
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
            }
        }

        #region Message
        public async Task HandleMessageUpdate(Message message)
        {
            var text = message.Text;

            if (text is null)
                return;
            switch (text.ToLower())
            {
                case "/start":
                    await HandleCommandStart(message);
                    break;
                case "використовувати за замовчуванням":
                    await SetDefaultLocation(message.Chat.Id);
                    break;
                case "надіслати своє положення":
                    await SetLocationFromMessage(message);
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
                default:
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


                await _client.SendMessage(message.Chat.Id, "Привіт, виберіть початкові налаштування", replyMarkup: keyBoard);
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
        private async Task SetDefaultLocation(long chatID)
        {
            await _settingService.GetSettingAsync(chatID, lat, lon);
            await _client.SendMessage(chatID, "Встановлено відстеження погоди для міста Київ");
            await SetMainButtons(chatID);
        }
        private async Task SetLocationFromMessage(Message message)
        {
            float latitude = (float)message.Location.Latitude;
            float longtitude = (float)message.Location.Longitude;

            var userSetting = await _settingService.GetSettingAsync(message.Id, latitude, longtitude);

            var locationName = await _geocodingService.GetNameAsync(userSetting.Latitude, userSetting.Longtitude);

            await _client.SendMessage(message.Chat.Id, $"Встановлено місце відстеження: {locationName}");

            await SetMainButtons(message.Chat.Id);
        }
        private async Task SetLocationFromName(Message message)
        {
            
            if (message.Text is null)
                return;

            var location = await _geocodingService.GetPointAsync(message.Text);

            if (location is null)
            {
                await _client.SendMessage(message.Chat.Id, "Проблема із назвою, не вийшло знайти");
                return;
            }

            var userSetting = await _settingService.GetSettingAsync(message.Chat.Id, location.Latitude, location.Longitude);

            var locationName = await _geocodingService.GetNameAsync(userSetting.Latitude, userSetting.Longtitude);

            await _client.SendMessage(message.Chat.Id, $"Встановлено місце відстеження {locationName}");
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

            var city = await _geocodingService.GetNameAsync(setting.Latitude, setting.Longtitude) ?? "Не отриано";


            var stringAnswer = "Користовацькі налаштування:" +
                "\nМісце відстеження: " + city +
                "\nКількість відстеження майбутніх днів: " + (setting.ForecastDays ?? 7) +
                "\nКалькість відстеження минулих днів: " + (setting.PastDays ?? 0) +
                "\nОдиниця вимірювання температури: " + (setting.TempUnit ?? "°C") +
                "\nОдиниця вимірювання швидкості: " + (setting.WindSpeed ?? "kh/s") +
                "\nБажаєте щось змінити?";

            var buttons = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("UserSettingCity")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("UserSettingForecat"),
                    InlineKeyboardButton.WithCallbackData("UserSettingPast")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("UserSettingTemperature"),
                    InlineKeyboardButton.WithCallbackData("UserSettingWind")
                }
            });

            await _client.SendMessage(message.Chat.Id, stringAnswer, replyMarkup: buttons);
        }

        #endregion

        #endregion
    }
}
