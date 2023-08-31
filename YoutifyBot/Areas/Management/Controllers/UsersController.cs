using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using YoutifyBot.Areas.Management.Models.ViewModels;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;

namespace YoutifyBot.Areas.Management.Controllers;

[Area("Management")]
public class UsersController : Controller
{
    IUnitOfWork unitOfWork;
    public UsersController(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

    public async Task<IActionResult> Index(int pageSize = 10, int pageIndex = 1)
    {
        var users = await unitOfWork.Repository<User>().GetAllAsync();

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
        var user = await unitOfWork.Repository<User>().FindByChatIdAsync(chatId);
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
        unitOfWork.Repository<User>().Update(user);
        await unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long chatId)
    {
        var user = await unitOfWork.Repository<User>().FindByChatIdAsync(chatId);
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
        unitOfWork.Repository<User>().Delete(new User { ChatId = userViewModel.ChatId });
        await unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> SendMessage(UsersViewModel user)
    {
        var viewModel = new SendMessageViewModel()
        {
            ChatId = user.ChatId,
            FullName =$"{user.FirstName} {user.LastName}"
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(SendMessageViewModel sendMessageViewModel)
    {
        if(ModelState.IsValid)
        {

        }
        return View();
    }
}
