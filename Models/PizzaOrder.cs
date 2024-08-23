using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kds.Models;

public class PizzaOrder
{
    public PizzaOrder()
    {
    }

    public PizzaOrder(string? orderId, string type, string flavor)
    {
        OrderId = orderId;
        Type = type;
        Title = type;
        DoughType = type;
        Flavors.Add(flavor);
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? OrderId { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string DoughType { get; set; }
    public List<string> Observation { get; set; } = new List<string>();
    public List<string> Flavors { get; set; } = new List<string>();
    public string? FlowId { get; set; }
    public string? StationId { get; set; }
    public IEnumerable<StartEndReport> StartEndReports { get; set; } = new List<StartEndReport>();
}
