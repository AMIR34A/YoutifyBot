using System.ComponentModel.DataAnnotations;

namespace YoutifyBot.Areas.Management.Models.ViewModels;

public class SendMessageViewModel
{
    public long ChatId { get; set; }
    public string FullName { get; set; }
    [Required]
    public string Text { get; set; }
    public string Media { get; set; }
}
