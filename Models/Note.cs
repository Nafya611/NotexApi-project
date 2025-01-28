
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Note
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required string Title { get; set; } // Title of the note
    public required string Content { get; set; } // Content of the note
    public DateTime CreatedAt { get; set; } // Date and time when the note was created
    public DateTime UpdatedAt { get; set; } // Date and time when the note was last updated
}
