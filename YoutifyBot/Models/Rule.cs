namespace YoutifyBot.Models;

public class Rule
{
    public int RuleId { get; set; }
    public int BaseDownloadSize { get; set; }
    public int MaximumDownloadSize { get; set; }
    public int AmountRewardInviting { get; set; }
    public bool ActiveNecessaryJoin { get; set; }
    public Role Access { get; set; }
}