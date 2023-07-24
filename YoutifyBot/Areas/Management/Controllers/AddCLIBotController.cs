using Microsoft.AspNetCore.Mvc;
using WTelegram;

namespace YoutifyBot.Areas.Management.Controllers;

[Area("Management")]
public class AddCLIBotController : Controller
{
    IConfiguration configuration;
    static IConfigurationSection configurationSections;
    public AddCLIBotController(IConfiguration configuration)
    {
        this.configuration = configuration;
        new ConfigurationManager().AddJsonFile("logininformations.json").Build().Reload();
        configurationSections = new ConfigurationBuilder().AddJsonFile("logininformations.json").Build().GetSection("profile");
    }
    public async Task<IActionResult> Index()
    {
        WTelegram.Client client = new Client(Config);
        await client.LoginUserIfNeeded();
        return View();
    }
    static string Config(string what)
    {
        switch (what)
        {
            case "api_id": return configurationSections["api_id"];
            case "api_hash": return configurationSections["api_hash"];
            case "phone_number": return configurationSections["phone_number"];
            case "verification_code":
                new ConfigurationManager().AddJsonFile("logininformations.json").Build().Reload();
                return configurationSections["verification_code"];
            //case "first_name": return configurationSections["first_name"];
            //case "last_name": return configurationSections["last_name"];
            default: return null;
        }
    }
}
