using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string InviteCode { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}