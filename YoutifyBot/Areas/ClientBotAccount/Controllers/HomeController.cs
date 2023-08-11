using Microsoft.AspNetCore.Mvc;

namespace YoutifyBot.Areas.ClientBotAccount.Controllers;

[Area("ClientBotAccount")]
public class HomeController : Controller
{
    CliBot cliBot;
    public HomeController(CliBot cliBot)
    {
        this.cliBot = cliBot;
    }
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string code)
    {
        await cliBot.LoginAsync(code);
        return RedirectToAction("Index", new { area = "YoutifyBot", controller = "Bot" });
    }
}
