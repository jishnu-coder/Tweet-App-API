using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tweet_App_API.Model
{
    public class Tweet
    {
        [JsonIgnore]
        [BsonId]
        public ObjectId id { get; set; }

        [BsonRequired]
        [BsonElement("tweetId")]
        public string TweetId { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("creatorId")]
        public string CreatorId { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonRequired]
        [BsonElement("createTime")]
        public DateTime CreateTime { get; set; }


        [BsonElement("tags")]
        public List<string> Tags { get; set; }

        [BsonElement("likes")]
        public List<string> Likes { get; set; }

        [BsonElement("replys")]
        public List<TweetReply> Replys { get; set; }
    }

    public class TweetReply
    {
        public string Replied_userId { get; set; }

        public string ReplyMessage { get; set; }

        public DateTime Reply_Time { get; set; }
    }

}
