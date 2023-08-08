namespace YoutifyBot.Models;

public class User
{
    public required long ChatId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public int MaximumSize { get; set; }
    public Role UserRole { get; set; }
}