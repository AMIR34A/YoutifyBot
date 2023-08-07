﻿namespace YoutifyBot.Models;

public class Rule
{
    public int RuleId { get; set; }
    public int BaseDownloadSize { get; set; }
    public int MaximumDownloadSize { get; set; }
    public Access GeneralAccess { get; set; }
}