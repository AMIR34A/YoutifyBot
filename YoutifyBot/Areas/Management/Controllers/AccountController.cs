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

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInViewModel logInViewModel)
        {
            if (!ModelState.IsValid)
                return NotFound();
            var user = await _userManager.FindByNameAsync(logInViewModel.Username);
            if (user is null)
                return NotFound();
            var signInResult = await _signInManager.PasswordSignInAsync(user, logInViewModel.Password, false, false);
            if (signInResult.Succeeded)
                return RedirectToAction("Index", "Users");
            ModelState.AddModelError(string.Empty, "The password is invalid!!!");
            return View();
        }
    }
}
