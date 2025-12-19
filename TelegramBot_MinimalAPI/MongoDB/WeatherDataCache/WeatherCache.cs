using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TelegramBot_MinimalAPI.Response;

namespace TelegramBot_MinimalAPI.MongoDB.WeatherDataCache
{
    public class WeatherCache
    {
        [BsonRepresentation(BsonType.ObjectId)] public string _id;
        [BsonElement("userId")] public long UserId { get; set; }
        [BsonElement("key")] public string Key { get; set; }
        [BsonElement("response")] public BaseResponse Cache { get; set; }


        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)][BsonElement("expiredTime")] 
        public DateTime ExpiredTime { get; set; } 
    }
}
