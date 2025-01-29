using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace NoteManagementSystem.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("Content")]
        public string? Content { get; set; }

        [BsonElement("CategoryId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CategoryId { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
    }
}
