using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kds.Models;

public class Invite
{
    public Invite(string code)
    {
        Code = code;
        ExpiresAt = DateTime.Now.AddHours(2);
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Code { get; set; }
    public Boolean Used { get; set; }
    public DateTime ExpiresAt { get; set; }
}