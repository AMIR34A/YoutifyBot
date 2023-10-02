using System.ComponentModel.DataAnnotations;
using YoutifyBot.Models;

namespace YoutifyBot.Areas.Management.Models.ViewModels;

public class RuleViewModel
{
    public int RuleId { get; set; }

    [Display(Name = "Base Download Size")]
    public int BaseDownloadSize { get; set; }

    [Display(Name = "Maximum Download Size")]
    public int MaximumDownloadSize { get; set; }

    [Display(Name = "Amount Reward Inviting")]
    public int AmountRewardInviting { get; set; }

    [Display(Name = "Neccessary Join")]
    public bool IsNecessaryJoinActive { get; set; }

    [Display(Name = "Necessary Join Channels")]
    public string? NecessaryJoinChannels { get; set; }

    [Display(Name = "Necessary Join For")]
    public Role NecessaryJoinFor { get; set; }
}
