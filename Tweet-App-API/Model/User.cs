using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Tweet_App_API.Model
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [JsonIgnore]
        [BsonId]
        public ObjectId id { get; set; } = ObjectId.GenerateNewId();

        [Required]
        [BsonRequired]
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("email")]
        public string Email { get; set; }

       
        [JsonIgnore]
        [BsonRequired]
        [BsonElement("loginId")]
        public string LoginId { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("password")]
        public string Password { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("contactNumber")]
        public string ContactNumber { get; set; }

    }

}
