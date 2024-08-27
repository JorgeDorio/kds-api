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

    public UserService(IOptions<Settings> settings, AuthService authService)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _invitesCollection = mongoDatabase.GetCollection<Invite>(settings.Value.InvitesCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(settings.Value.UsersCollectionName);
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

        var user = await _usersCollection.Find(x => x.Username == newUser.Username).FirstOrDefaultAsync();
        if (user != null) return new UnprocessableEntityObjectResult("Usuário já cadastrado");

        newUser.Password = _authService.Hash(newUser.Password);
        await _usersCollection.InsertOneAsync(newUser);
        await _invitesCollection.FindOneAndUpdateAsync(x => x.Id == invite.Id, Builders<Invite>.Update.Set(i => i.Used, true));

        return new OkObjectResult("Usuário cadastrado com sucesso");
    }

    public async Task<IActionResult> LoginAsync(User user)
    {
        var dbUser = await _usersCollection.Find(x => x.Username == user.Username).FirstOrDefaultAsync();
        if (dbUser == null || !_authService.CompareHash(user.Password, dbUser.Password)) return new NotFoundObjectResult("Usuário ou senha inválidos");

        var token = _authService.GenerateToken(dbUser);

        return new OkObjectResult(token);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _usersCollection.Find(x => true).ToListAsync();

        return users;
    }
}