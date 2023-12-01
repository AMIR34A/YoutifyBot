using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YoutifyBot.Areas.Management.Models.ViewModels;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;
using User = YoutifyBot.Models.User;

namespace YoutifyBot.Areas.Management.Controllers;

[Area("Management")]
public class UsersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser<string>> _userManager;

    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser<string>> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageSize = 10, int pageIndex = 1)
    {
        var users = await _unitOfWork.Repository<User>().GetAllAsync();

        var paging = PagingList.Create(users.Select(
            user => new UsersViewModel
            {
                ChatId = user.ChatId,
                FirstName = string.IsNullOrEmpty(user.FirstName) ? "-" : user.FirstName,
                LastName = string.IsNullOrEmpty(user.LastName) ? "-" : user.LastName,
                Username = string.IsNullOrEmpty(user.Username) ? "-" : user.Username,
                MaximumDownloadSize = user.MaximumDownloadSize,
                TotalDonwload = user.TotalDonwload,
                UserRole = user.UserRole
            }), pageSize, pageIndex);

        paging.RouteValue = new RouteValueDictionary
        {
            { "pagesize" , pageSize }
        };
        return View(paging);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long chatId)
    {
        var user = await _unitOfWork.Repository<User>().FindByChatIdAsync(chatId);
        var userViewModel = new UsersViewModel()
        {
            ChatId = user.ChatId,
            FirstName = string.IsNullOrEmpty(user.FirstName) ? "-" : user.FirstName,
            LastName = string.IsNullOrEmpty(user.LastName) ? "-" : user.LastName,
            Username = string.IsNullOrEmpty(user.Username) ? "-" : user.Username,
            MaximumDownloadSize = user.MaximumDownloadSize,
            TotalDonwload = user.TotalDonwload,
            UserRole = user.UserRole
        };
        return View(userViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsersViewModel userViewModel)
    {
        var identityUser = await _userManager.FindByNameAsync(userViewModel.Username);
        if (identityUser is null)
            return NotFound();
        if (userViewModel.UserRole == Role.Admin)
            await _userManager.AddToRoleAsync(identityUser, "Admin");
        else
            await _userManager.RemoveFromRoleAsync(identityUser, "Admin");


        var user = new User()
        {
            ChatId = userViewModel.ChatId,
            FirstName = userViewModel.FirstName,
            LastName = userViewModel.LastName,
            Username = userViewModel.Username,
            MaximumDownloadSize = userViewModel.MaximumDownloadSize,
            TotalDonwload = userViewModel.TotalDonwload,
            UserRole = userViewModel.UserRole
        };
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long chatId)
    {
        var user = await _unitOfWork.Repository<User>().FindByChatIdAsync(chatId);
        var userViewModel = new UsersViewModel()
        {
            ChatId = user.ChatId,
            FirstName = string.IsNullOrEmpty(user.FirstName) ? "-" : user.FirstName,
            LastName = string.IsNullOrEmpty(user.LastName) ? "-" : user.LastName,
        };

        return View(userViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(UsersViewModel userViewModel)
    {
        _unitOfWork.Repository<User>().Delete(new User { ChatId = userViewModel.ChatId });
        await _unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> SendMessage(UsersViewModel user)
    {
        var viewModel = new SendMessageViewModel()
        {
            ChatId = user.ChatId,
            FullName = $"{user.FirstName} {user.LastName}"
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(SendMessageViewModel sendMessageViewModel)
    {
        if (ModelState.IsValid)
        {
            TelegramBotClient telegramBotClient = new TelegramBotClient("6398637615:AAH5-cUXSMSzYjWTS1gqYxKR52w-FjuhMZA");

            if (string.IsNullOrEmpty(sendMessageViewModel.Media))
                await telegramBotClient.SendTextMessageAsync(sendMessageViewModel.ChatId, sendMessageViewModel.Text, parseMode: ParseMode.Html);
            else
                await telegramBotClient.SendPhotoAsync(sendMessageViewModel.ChatId, new InputFileId(sendMessageViewModel.Media),
                    caption: sendMessageViewModel.Text, parseMode: ParseMode.Html);

            return RedirectToAction("Index");
        }
        return View(sendMessageViewModel);
    }
}
