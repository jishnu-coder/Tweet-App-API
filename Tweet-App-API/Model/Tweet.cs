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
        public Creator Creator { get; set; }

        [Required]
        [BsonRequired]
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonRequired]
        [BsonElement("createTime")]
        public DateTime CreateTime { get; set; }

        public string DateTimeStamp { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; }

        [BsonElement("likes")]
        public List<string> Likes { get; set; }

        [BsonElement("replys")]
        public List<TweetReply> Replys { get; set; }
    }

    public class TweetReply
    {
        [Required]
        public string Replied_userId { get; set; }

        [Required]
        public string ReplyMessage { get; set; }

        public DateTime Reply_Time { get; set; }

        public string Reply_Time_Stamp { get; set; }

        public List<string> ReplyTags { get; set; }
    }

    public class Creator
    {
        [Required]
        public string CreatorId { get; set; }

        
        public int Seq { get; set; }
    }

}
