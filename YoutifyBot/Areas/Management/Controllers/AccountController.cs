using Microsoft.AspNetCore.Mvc;

namespace YoutifyBot.Areas.Management.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
