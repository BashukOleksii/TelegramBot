using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);


var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");

if (string.IsNullOrWhiteSpace(botToken))
    throw new Exception("Токен не отриманий");

builder.Services.AddSingleton(new TelegramBotClient(botToken));

var app = builder.Build();

app.MapPost("/telegram/update", async (Update update, TelegramBotClient botClient) =>
{
    if (update.Message?.Text is not null)
    {
        await botClient.SendMessage(update.Message.Chat.Id, "Привіт!");
    }
});

app.Run();
