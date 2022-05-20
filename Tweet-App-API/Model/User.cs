using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
namespace Tweet_App_API.Model
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public ObjectId id { get; set; } = ObjectId.GenerateNewId();

        [BsonRequired]
        [BsonElement("firstName")]
        public string FirstName { get; set; }


        [BsonRequired]
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonRequired]
        [BsonElement("email")]
        public string Email { get; set; }

        [JsonIgnore]
        [BsonRequired]
        [BsonElement("loginId")]
        public string LoginId { get; set; }


        [BsonRequired]
        [BsonElement("password")]
        public string Password { get; set; }

        [BsonRequired]
        [BsonElement("contactNumber")]
        public string ContactNumber { get; set; }

    }

}
