namespace YoutifyBot.Models;

public class Rule
{
    public int RuleId { get; set; }
    public int BaseDownloadSize { get; set; }
    public int MaximumDownloadSize { get; set; }
    public int AmountRewardInviting { get; set; }
    public bool IsNecessaryJoinActive { get; set; }
    public string? NecessaryJoinChannels { get; set; }
    public Role NecessaryJoinFor { get; set; }
}