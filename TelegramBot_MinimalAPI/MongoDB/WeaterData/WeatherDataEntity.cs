using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.MongoDB.WeaterData
{
    public class WeatherDataEntity
    {

        [BsonRepresentation(BsonType.ObjectId)] public string _id {  get; set; }
        [BsonElement("userId")] public long  UserId { get; set; }
        [BsonElement("hourlyArray")] public List<string>? HourlyArray { get; set; }
        [BsonElement("hourlyIndex")] public int? HourlyIndex { get; set; }
        [BsonElement("dailyArray")] public List<string>? DailyArray { get; set; }
        [BsonElement("dailyIndex")] public int? DailyIndex { get; set; }

    }
}
