using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Kds;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class AuthService
{
    private readonly string _privateKey;
    public AuthService(IOptions<Settings> settings)
    {
        _privateKey = settings.Value.PrivateKey;
    }
    public string Hash(string value)
    {
        var sha1 = SHA1.Create();

        var bytes = new ASCIIEncoding().GetBytes(value);

        bytes = sha1.ComputeHash(bytes);

        var stringHash = new StringBuilder();

        foreach (var b in bytes) stringHash.Append(b.ToString("x2"));

        return stringHash.ToString();
    }

    public bool CompareHash(string value, string hash)
    {
        string hashedValue = Hash(value);

        return hashedValue.Equals(hash, StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_privateKey);

        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(8),
            Subject = GenerateClaims(user)
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();

        ci.AddClaim(new Claim("username", user.Username));
        ci.AddClaim(new Claim("userId", user.Id.ToString()));
        ci.AddClaim(new Claim("isAdmin", user.Admin.ToString()));

        return ci;
    }
}