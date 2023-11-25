using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YoutifyBot.Areas.Management.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LogIn() => View();
    }
}
