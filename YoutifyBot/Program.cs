using WTelegram;
using YoutifyBot.Areas;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<YoutifyBotContext>();
string path = Directory.GetCurrentDirectory() + "\\WTelegram.session";
var sections = builder.Configuration.AddJsonFile("logininformations.json").Build().GetSection("profile");
builder.Services.AddSingleton(new Client(int.Parse(sections["api_id"]), sections["api_hash"], path));
builder.Services.AddSingleton<CliBot>();
//builder.Services.AddDbContext<YoutifyBotContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("YoutifyConnection")));

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "youtifybot_route",
    areaName: "YoutifyBot",
    pattern: "YoutifyBot/{controller}/{action}");

app.MapAreaControllerRoute(
    name: "management_route",
    areaName: "Management",
    pattern: "Management/{controller}/{action}");

app.MapAreaControllerRoute(
    name: "clientbotaccount_route",
    areaName: "ClientBotAccount",
    pattern: "ClientBotAccount/{controller}/{action}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();