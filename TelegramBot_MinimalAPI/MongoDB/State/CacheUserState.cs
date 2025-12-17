using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot_MinimalAPI.MongoDB.State
{
    public class CacheUserState
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("userId")]
        public long UserId { get; set; }
        [BsonElement("userState")]
        public UserStates UserStates { get; set; }
    }
}
