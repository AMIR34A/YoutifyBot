using Microsoft.AspNetCore.Mvc;
using YoutifyBot.Areas.Management.Models.ViewModels;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;

namespace YoutifyBot.Areas.Management.Controllers;

[Area("Management")]
public class RuleController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public RuleController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IActionResult> Index()
    {
        var rule = await _unitOfWork.Repository<Rule>().GetFirstAsync();
        var ruleViewModel = new RuleViewModel
        {
            RuleId = rule.RuleId,
            BaseDownloadSize = rule.BaseDownloadSize,
            MaximumDownloadSize = rule.MaximumDownloadSize,
            AmountRewardInviting = rule.AmountRewardInviting,
            IsNecessaryJoinActive = rule.IsNecessaryJoinActive,
            NecessaryJoinChannels = rule.NecessaryJoinChannels,
            NecessaryJoinFor = rule.NecessaryJoinFor
        };
        return View(ruleViewModel);
    }
}
