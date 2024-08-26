using System.Security.Cryptography;
using System.Text;

public class AuthService{
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
}