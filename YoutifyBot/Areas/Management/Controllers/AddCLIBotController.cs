using Microsoft.AspNetCore.Mvc;

namespace YoutifyBot.Areas.Management.Controllers;

[Area("Management")]
public class AddCLIBotController : Controller
{
    public async Task<IActionResult> Index()
    {
        CliBot cliBot = new CliBot();
        await cliBot.LoginAsync();
        return View();
    }
}
