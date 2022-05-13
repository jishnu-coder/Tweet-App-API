using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Tweet_App_API.Model
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public ObjectId id { get; set; }

        [BsonRequired]
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        
        [BsonRequired]
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonRequired]
        [BsonElement("email")]
        public string Email { get; set; }

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
