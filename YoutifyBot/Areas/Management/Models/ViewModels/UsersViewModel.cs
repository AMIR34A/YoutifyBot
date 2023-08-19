using System.ComponentModel.DataAnnotations;
using YoutifyBot.Models;

namespace YoutifyBot.Areas.Management.Models.ViewModels;

public class UsersViewModel
{
    public required long ChatId { get; set; }
    [Display(Name ="First name")]
    public string? FirstName { get; set; }
    [Display(Name = "Last name")]
    public string? LastName { get; set; }
    [Display(Name = "Username")]
    public string? Username { get; set; }
    [Display(Name = "Maximum Download")]
    public int MaximumDownloadSize { get; set; }
    [Display(Name = "Total Donwload")]
    public int TotalDonwload { get; set; }
    [Display(Name = "Role")]
    public Role UserRole { get; set; }
}
