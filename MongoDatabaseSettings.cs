namespace Kds;

public class MongoDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string StationsCollectionName { get; set; } = null!;

    public string OrdersCollectionName { get; set; } = null!;
    public string PizzaOrdersCollectionName { get; set; } = null!;
    public string FlowsCollectionName { get; set; } = null!;
}