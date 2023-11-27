using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YoutifyBot.Models.ViewModels;

namespace YoutifyBot.Areas.Management.Controllers
{
    [Area("Management")]
    public class AccountController : Controller
    {
        UserManager<IdentityUser> _userManager;
        SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult LogIn() => View();
    }
}
