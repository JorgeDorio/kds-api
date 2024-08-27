using Kds.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kds.Service;

public class StationService
{
    private readonly IMongoCollection<Station> _stationsCollection;

    public StationService(IOptions<Settings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _stationsCollection = mongoDatabase.GetCollection<Station>(settings.Value.StationsCollectionName);
    }

    public async Task CreateAsync(Station newStation)
    {
        await _stationsCollection.InsertOneAsync(newStation);
    }

    public async Task<IEnumerable<Station>> GetAsync()
    {
        return await _stationsCollection.Find(_ => true).ToListAsync();
    }
}