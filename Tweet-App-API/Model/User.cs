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
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        
        [BsonElement("loginId")]
        public string LoginId { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("contactNumber")]
        public string ContactNumber { get; set; }
       
    }
}
