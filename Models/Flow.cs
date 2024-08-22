using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kds.Models;

public class Flow
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Name { get; set; }

    public required IEnumerable<string> Stations { get; set; }
}