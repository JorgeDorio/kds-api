using Kds.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kds.Service;

public class UserService
{
    private readonly IMongoCollection<Invite> _invitesCollection;

    public UserService(IOptions<MongoDatabaseSettings> dbSettings)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _invitesCollection = mongoDatabase.GetCollection<Invite>(dbSettings.Value.InvitessCollectionName);
    }

    public async Task<string> GenerateInviteCode()
    {
        var random = new Random();
        var length = 6;
        var number = random.Next(0, (int)Math.Pow(10, length));
        var code = number.ToString($"D{length}");

        var invite = new Invite(code);

        await _invitesCollection.InsertOneAsync(invite);

        return code;
    }
}