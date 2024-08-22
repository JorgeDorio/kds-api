using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kds.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonIgnore]
    public required string Data { get; set; }
}