using System.Reflection.Metadata;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot_MinimalAPI.GeocodingAndReverseService;
using TelegramBot_MinimalAPI.MongoDB.Service.Interfaces;

namespace TelegramBot_MinimalAPI
{
    
    public class UpdateHandler
    {
        private readonly TelegramBotClient _client;
        private readonly ISettingService _settingService;
        private readonly GeocodingServise _geocodingService;


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
                    if(update.Message is not null)
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
                    await HandleCommandStart(message.Chat.Id);
                break;
                case "налаштування":
                    await HandleSettingButton(message.Chat.Id);
                    break;
                case "<-- назад":
                    await HandleCommandStart(message.Chat.Id);
                    break;
                case "користувацькі налаштування":
                    await HandleUserSettingButton(message);
                break;
            }
            

            
        }

        private async Task HandleCommandStart(long chatID)
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
                ResizeKeyboard = true ,
                OneTimeKeyboard = true
            };
      
            await _client.SendMessage(chatID, "Привіт, вибери дію", replyMarkup: keyBoard);
        }

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
            float lat = (float)50.45466, lon = (float)30.5238;

            if (message.Location!= null)
            {
                lat = (float)message.Location.Latitude;
                lon = (float)message.Location.Longitude;
            }

            var setting = await _settingService.GetSettingAsync(message.From.Id, lat, lon);

            if(setting is null)
            {
                await _client.SendMessage(message.Chat.Id, "Помилка отримання інформації");
                return;
            }

            var city = await _geocodingService.GetNameAsync(setting.Latitude, setting.Longtitude) ?? "Не отриано";

           
            var stringAnswer = "Користовацькі налаштування:" +
                "\nМісце відстеження: " + city +
                "\nКількість відстеження майбутніх днів: " + (setting.ForecastDays ?? 7) +
                "\nКалькість відстеження минулих днів" + (setting.PastDays ?? 0) +
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
    }
}
