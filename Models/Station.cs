using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kds.Models;

public class Station
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<PizzaOrder>? Orders { get; set; }
    public PizzaOrder? NextOrder { get; set; }
}