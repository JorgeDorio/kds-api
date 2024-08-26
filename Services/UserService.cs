using Kds.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kds.Service;

public class UserService
{
    private readonly IMongoCollection<Invite> _invitesCollection;
    private readonly IMongoCollection<User> _usersCollection;
    private readonly AuthService _authService;

    public UserService(IOptions<MongoDatabaseSettings> dbSettings, AuthService authService)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _invitesCollection = mongoDatabase.GetCollection<Invite>(dbSettings.Value.InvitesCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(dbSettings.Value.UsersCollectionName);
        _authService = authService;
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

    public async Task<IActionResult> CreateAsync(User newUser)
    {
        var invite = await _invitesCollection.Find(x => x.Code == newUser.InviteCode && x.ExpiresAt > DateTime.Now && !x.Used).FirstOrDefaultAsync();
        if (invite == null) return new NotFoundObjectResult("Código de convite inválido ou expirado");
        newUser.Password = _authService.Hash(newUser.Password);
        await _usersCollection.InsertOneAsync(newUser);
        await _invitesCollection.FindOneAndUpdateAsync(x => x.Id == invite.Id, Builders<Invite>.Update.Set(i => i.Used, true));

        return new OkObjectResult("Usuário cadastrado com sucesso");
    }
}