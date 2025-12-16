using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot_MinimalAPI;
using TelegramBot_MinimalAPI.MongoDB.Repository.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.Repository.Realizations;
using TelegramBot_MinimalAPI.MongoDB.Service.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.Service.Realizations;
var builder = WebApplication.CreateBuilder(args);

#region DI
#region BotToken
var botToken = "8481914772:AAHaSlwTaRZd7yXMo6nu_DYcrXOW0JnARKk"; //Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrWhiteSpace(botToken))
    throw new Exception("Токен не отриманий");
builder.Services.AddSingleton(new TelegramBotClient(botToken));
#endregion

#region MongoDB
var connectionString = "mongodb+srv://bashuk0325oleksij_db_user:lZHXFstos2k8lAMX@data.t7bzerb.mongodb.net/?retryWrites=true&w=majority&appName=Data";
                  
builder.Services.AddSingleton(new MongoClient(connectionString));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<MongoClient>();
    return client.GetDatabase("Setting");
});
builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
builder.Services.AddSingleton<ISettingService, SettingService>();
#endregion

#region UpdateHandler
builder.Services.AddSingleton<UpdateHandler>();
#endregion

#endregion

var app = builder.Build();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//app.Urls.Add($"http://0.0.0.0:{port}");

app.MapPost("/telegram/update", async (Update update, UpdateHandler updateHandler) =>
{
    await updateHandler.HandleUpdate(update);
    return Results.Ok();
});

app.Run();
