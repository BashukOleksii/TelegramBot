using System.Reflection.Metadata;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot_MinimalAPI
{
    public class UpdateHandler
    {
        private readonly TelegramBotClient _client;

        public UpdateHandler(TelegramBotClient client)
        {
            _client = client;
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
                case "Налаштування":
                    break;
            }
            

            
        }

        public async Task HandleCommandStart(long chatID)
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

        #endregion
    }
}
