using Kds.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kds.Service;

public class FlowService
{
    private readonly IMongoCollection<Flow> _flowsCollection;

    public FlowService(IOptions<Settings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _flowsCollection = mongoDatabase.GetCollection<Flow>(settings.Value.FlowsCollectionName);
    }

    public async Task CreateAsync(Flow newFlow)
    {
        await _flowsCollection.InsertOneAsync(newFlow);
    }

    public async Task<IEnumerable<Flow>> GetAsync()
    {
        return await _flowsCollection.Find(_ => true).ToListAsync();
    }
}