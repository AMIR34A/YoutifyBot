﻿using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<IActionResult> Edit(int ruleId)
    {
        var rule = await _unitOfWork.Repository<Rule>().FindByRuleIdAsync(ruleId);
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

    [HttpPost]
    public async Task<IActionResult> Edit(RuleViewModel ruleViewModel)
    {
        var rule = new Rule
        {
            RuleId = ruleViewModel.RuleId,
            BaseDownloadSize = ruleViewModel.BaseDownloadSize,
            MaximumDownloadSize = ruleViewModel.MaximumDownloadSize,
            AmountRewardInviting = ruleViewModel.AmountRewardInviting,
            IsNecessaryJoinActive = ruleViewModel.IsNecessaryJoinActive,
            NecessaryJoinChannels = ruleViewModel.NecessaryJoinChannels,
            NecessaryJoinFor = ruleViewModel.NecessaryJoinFor
        };

        _unitOfWork.Repository<Rule>().Update(rule);
        await _unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int ruleId)
    {
        var rule = await _unitOfWork.Repository<Rule>().FindByRuleIdAsync(ruleId);
        _unitOfWork.Repository<Rule>().Delete(rule);
        return RedirectToAction("Index");
    }
}