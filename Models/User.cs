using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class User
{
    [BsonId] 
    public ObjectId Id { get; set; } 

    public required string Username { get; set; }
    public required string Password { get; set; }
}
