namespace YoutifyBot.Models;

public class Rule
{
    public int RuleId { get; set; }
    public int BaseDownloadVolume { get; set; }
    public int MaximumDownloadVolume { get; set; }
    public int BaseCountDownload { get; set; }
}
