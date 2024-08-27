namespace Kds;

public class Settings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string StationsCollectionName { get; set; } = null!;
    public string OrdersCollectionName { get; set; } = null!;
    public string PizzaOrdersCollectionName { get; set; } = null!;
    public string FlowsCollectionName { get; set; } = null!;
    public string InvitesCollectionName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
}