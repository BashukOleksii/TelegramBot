using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot_MinimalAPI.GeocodingAndReverseService;
using TelegramBot_MinimalAPI.MongoDB.Setting.Repository.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.Setting.Repository.Realizations;
using TelegramBot_MinimalAPI.MongoDB.Setting.Service.Interfaces;
using TelegramBot_MinimalAPI.MongoDB.Setting.Service.Realizations;
using TelegramBot_MinimalAPI.MongoDB.State.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.State.Repository.Realization;
using TelegramBot_MinimalAPI.MongoDB.State.Service.Interface;
using TelegramBot_MinimalAPI.MongoDB.State.Service.Realization;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Repository.Realization;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeaterData.Service.Realization;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Repository.Realization;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Service.Interface;
using TelegramBot_MinimalAPI.MongoDB.WeatherDataCache.Service.Realization;
using TelegramBot_MinimalAPI.UpdateHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsetings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine(builder.Environment.EnvironmentName);

#region DI
#region BotToken
var botToken = builder.Configuration["Telegram:BotToken"]; //Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrWhiteSpace(botToken))
    throw new Exception("Òîêåí íå îòðèìàíèé");
var telegramBot = new TelegramBotClient(botToken);
await telegramBot.SetMyCommands(new[]
{
    new BotCommand("start","Ïî÷àòîê ðîáîòè")
});
builder.Services.AddSingleton(telegramBot);
#endregion

#region MongoDB
var connectionString = builder.Configuration["MongoDB:ConnectionString"];
                  
builder.Services.AddSingleton(new MongoClient(connectionString));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<MongoClient>();
    return client.GetDatabase("Setting");
});
builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
builder.Services.AddSingleton<ISettingService, SettingService>();
builder.Services.AddSingleton<IStateRepository, StateRepository>();
builder.Services.AddSingleton<IStateService, StateService>();
builder.Services.AddSingleton<IWeatherDataRepository, WeatherDataRepository>();
builder.Services.AddSingleton<IWeatherDataService, WeatherDataService>();
builder.Services.AddSingleton<IWeatherCacheRepository, WeatherCacheRepository>();
builder.Services.AddSingleton<IWeatherCacheService, WeatherCacheService>();

#endregion

#region UpdateHandler
builder.Services.AddSingleton<UpdateHandler>();
builder.Services.AddHttpClient<GeocodingServise>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherBot/1.0 (bashuk0325oleksij@gmail.com)");

    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
});

#endregion

#endregion



var app = builder.Build();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
//app.Urls.Add($"http://0.0.0.0:{port}");

app.MapPost("/telegram/update", async (Update update, UpdateHandler updateHandler) =>
{
    await updateHandler.HandleUpdate(update);
    return Results.Ok();
});

app.Run();
