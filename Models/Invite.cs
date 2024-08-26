namespace Kds.Models;

public class Invite
{
    public Invite(string code)
    {
        Code = code;
        ExpiresAt = DateTime.Now.AddHours(2);
    }

    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
}