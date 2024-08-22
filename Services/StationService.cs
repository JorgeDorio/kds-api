using Kds.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kds.Service;

public class StationService
{
    private readonly IMongoCollection<Station> _stationsCollection;

    public StationService(IOptions<MongoDatabaseSettings> dbSettings)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

        _stationsCollection = mongoDatabase.GetCollection<Station>(dbSettings.Value.StationsCollectionName);
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