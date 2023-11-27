using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YoutifyBot.Areas.Management.Controllers
{
    [Area("Management")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult LogIn() => View();
    }
}
