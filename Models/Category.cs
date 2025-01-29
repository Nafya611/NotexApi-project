using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoteManagementSystem.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
    }
}
