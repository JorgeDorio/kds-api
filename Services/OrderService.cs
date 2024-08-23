using System.Text.RegularExpressions;
using Kds.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kds.Service;

public class OrderService
{
    private readonly IMongoCollection<PizzaOrder> _pizzaOrdersCollection;
    private readonly IMongoCollection<Order> _ordersCollection;
    private readonly IMongoCollection<Station> _stationsCollection;
    private readonly IMongoCollection<Flow> _flowsCollection;
    private readonly FlowService _flowService;

    public OrderService(IOptions<MongoDatabaseSettings> dbSettings, FlowService flowService)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _pizzaOrdersCollection = mongoDatabase.GetCollection<PizzaOrder>(dbSettings.Value.PizzaOrdersCollectionName);
        _ordersCollection = mongoDatabase.GetCollection<Order>(dbSettings.Value.OrdersCollectionName);
        _stationsCollection = mongoDatabase.GetCollection<Station>(dbSettings.Value.StationsCollectionName);
        _flowsCollection = mongoDatabase.GetCollection<Flow>(dbSettings.Value.FlowsCollectionName);
        _flowService = flowService;
    }

    public async Task CreateAsync(Order newOrder)
    {
        var orders = new List<PizzaOrder>();
        await _ordersCollection.InsertOneAsync(newOrder);
        if (newOrder.Data.Contains("TEMPLATE=pedido de entrega")) ParseDeliveryOrderTemplate(orders, newOrder.Id, newOrder.Data);
        else if (newOrder.Data.Contains("TEMPLATE=remota")) ParseRemoteTemplate(orders, newOrder.Id, newOrder.Data);

        var flows = await _flowService.GetAsync();

        if (orders.Any())
        {
            orders.ForEach(async x =>
            {
                var flow = flows.FirstOrDefault(y => y.Name.ToUpper().Contains(x.Type));
                x.FlowId = flow?.Id;
                x.StationId = flow?.Stations.First();
                await _pizzaOrdersCollection.InsertOneAsync(x);
            });
        }
    }

    public async Task<Station> GetByStationId(string stationId)
    {
        var station = (await _stationsCollection.FindAsync(x => x.Id == stationId)).FirstOrDefault();
        station.Orders = (await _pizzaOrdersCollection.FindAsync(x => x.StationId == stationId)).ToList();

        return station;
    }

    public async Task<Station> GetNextByStationId(string stationId)
    {
        var station = (await _stationsCollection.FindAsync(x => x.Id == stationId)).FirstOrDefault();
        // station.NextOrder = (await _pizzaOrdersCollection.FindAsync(x => x.StationId == stationId)).FirstOrDefault();
        station.NextOrder = (await _pizzaOrdersCollection.FindAsync(x => x.StationId == stationId && (
            x.StartEndReports.FirstOrDefault(y => y.StationId == stationId).EndedAt == null || !x.StartEndReports.Any(y => y.StationId == x.StationId)
        ))).FirstOrDefault();

        return station;
    }

    public async Task<IActionResult> StartOrder(string orderId)
    {
        var order = (await _pizzaOrdersCollection.FindAsync(x => x.Id == orderId)).FirstOrDefault();
        if (order.StartEndReports.FirstOrDefault(x => x.StationId == order.StationId) == null)
        {
            var report = order.StartEndReports.ToList();
            report.Add(new StartEndReport(order.StationId));
            await _pizzaOrdersCollection.FindOneAndUpdateAsync(x => x.Id == orderId, Builders<PizzaOrder>.Update.Set(o => o.StartEndReports, report));
            return new OkObjectResult("Pedido iniciado");
        }
        else return new UnprocessableEntityObjectResult("Pedido ja iniciado"); // Specify the error message

    }

    public async Task EndOrder(string orderId)
    {
        var order = (await _pizzaOrdersCollection.FindAsync(x => x.Id == orderId)).FirstOrDefault();
        var flow = (await _flowsCollection.FindAsync(x => x.Id == order.FlowId)).FirstOrDefault();

        var currentStationIndex = flow.Stations.ToList().IndexOf(order.StationId);
        var nextStationId = flow.Stations.Skip(currentStationIndex + 1).FirstOrDefault();

        order.StartEndReports.FirstOrDefault(x => x.StationId == order.StationId).EndedAt = DateTime.Now;

        await _pizzaOrdersCollection.FindOneAndUpdateAsync(x => x.Id == orderId, Builders<PizzaOrder>.Update
            .Set(o => o.StartEndReports, order.StartEndReports)
            .Set(o => o.StationId, nextStationId));
    }

    public static void ParseRemoteTemplate(List<PizzaOrder> orders, string? orderId, string input)
    {
        var pizzaOrder = new PizzaOrder()
        {
            OrderId = orderId
        };

        var splitedOrder = input.Split("Venda:");

        var orderData = splitedOrder[splitedOrder.Length - 1].Split("<EXPA>").Where(x => x.Contains("<NEXP>"));

        foreach (var line in orderData)
        {
            if (!line.Contains("<NCOM>")) pizzaOrder.Title = line.Split("<NEXP>")[0];
            else if (line.Split("<NCOM>")[0].ToUpper().Contains("FANTA")) continue;
            else
            {
                pizzaOrder.Flavors.Add(line.Split("<NCOM>")[0]);
            }
        }

        if (pizzaOrder.Flavors.Any(x => x.ToUpper().Contains("ESFIHA")))
        {
            pizzaOrder.Title = "ESFIHA";
            pizzaOrder.DoughType = "ESFIHA";
        }

        if (!pizzaOrder.Flavors.Any(x => x.ToUpper().Contains("PIZZA") || x.ToUpper().Contains("ESFIHA"))) return;

        if (pizzaOrder.Title.ToUpper().Contains("PIZZA")) pizzaOrder.Type = "PIZZA";
        else if (pizzaOrder.Title.ToUpper().Contains("ESFIHA")) pizzaOrder.Type = "ESFIHA";
        else pizzaOrder.Type = "OUTRO";

        orders.Add(pizzaOrder);
    }

    public static void ParseDeliveryOrderTemplate(List<PizzaOrder> orders, string? orderId, string input)
    {
        var splitedOrder = input.Split("----------------------------------------");

        var dataIndex = 0;
        for (var i = 0; i < splitedOrder.Length; i++)
        {
            if (splitedOrder[i].Contains("Dados do pedido")) dataIndex = i + 1;
        }

        var data = splitedOrder[dataIndex].Split("\n")
        .Where(item => !string.IsNullOrWhiteSpace(item)
        && !item.ToUpper().Contains("COCA COLA")
        && !item.ToUpper().Contains("COCA ZERO")
        && !item.ToUpper().Contains("FANTA")
        && !item.ToUpper().Contains("GUARANA")).ToArray();


        for (var i = 0; i < data.Length; i++)
        {
            if (!data[i].Contains("  "))
            {
                if (data[i].ToUpper().Contains("COMBO PIZZA") || data[i].ToUpper().Contains("COMBO ESFIHA"))
                {
                    var order = new PizzaOrder()
                    {
                        Title = data[i],
                        OrderId = orderId
                    };

                    for (var j = i + 1; j < data.Length; j++)
                    {
                        var line = data[j];
                        if (line.Contains("  "))
                        {
                            if (line.Contains("MASSA")) order.DoughType = GetDoughType(line);
                            else if (line.Contains(">>")) order.Observation.Add(line.Replace("  >> ", ""));
                            else order.Flavors.Add(line.Replace("  ", ""));
                        }
                        else
                        {
                            break;
                        };
                    }
                    if (order.Title.ToUpper().Contains("PIZZA")) order.Type = "PIZZA";
                    else if (order.Title.ToUpper().Contains("ESFIHA")) order.Type = "ESFIHA";
                    else order.Type = "OUTRO";
                    orders.Add(order);
                }
                else if (data[i].ToUpper().Contains("ESFIHA")) orders.Add(new(orderId, "ESFIHA", data[i]));
            }
            else continue;
        }
    }

    public static string GetDoughType(string line)
    {
        if (line.Contains("TRADICIONAL")) return "TRADICIONAL";
        return "TRADICIONAL";
    }
}

