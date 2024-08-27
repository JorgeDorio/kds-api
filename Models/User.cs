using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? InviteCode { get; set; }
    public string? FullName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool Admin { get; set; }
}