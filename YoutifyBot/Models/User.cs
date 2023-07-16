namespace YoutifyBot.Models;

public class User
{
    public required long ChatId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public  string? Username { get; set; }
}
