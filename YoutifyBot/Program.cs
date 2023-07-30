using WTelegram;
using YoutifyBot.Areas;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<YoutifyBotContext>();
builder.Services.AddSingleton<CliBot>();
builder.Services.AddSingleton(new Client(CliBot.Config));
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
