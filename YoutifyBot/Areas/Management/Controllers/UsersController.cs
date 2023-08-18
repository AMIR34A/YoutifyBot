using Microsoft.AspNetCore.Mvc;
using YoutifyBot.Models.Repository;

namespace YoutifyBot.Areas.Management.Controllers;

public class UsersController : Controller
{
    IUnitOfWork unitOfWork;
    public UsersController(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        return View();
    }
}
