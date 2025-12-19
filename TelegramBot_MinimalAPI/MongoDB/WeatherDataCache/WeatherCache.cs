using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TelegramBot_MinimalAPI.Response;

namespace TelegramBot_MinimalAPI.MongoDB.WeatherDataCache
{
    public class WeatherCache
    {
        [BsonRepresentation(BsonType.ObjectId)] public string _id;
        [BsonElement("userId")] public long UserId { get; set; }
        [BsonElement("key")] string Key { get; set; }
        [BsonElement("response")] BaseResponse Response { get; set; }
        [BsonRepresentation(BsonType.String)][BsonElement("expiredTime")] DateTime ExpiredTime { get; set; } 
    }
}
