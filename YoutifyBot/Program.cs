using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YoutifyBot.Areas;
using YoutifyBot.Data;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("YoutifyBotContextConnection") ?? throw new InvalidOperationException("Connection string 'YoutifyBotContextConnection' not found.");

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<YoutifyBotContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<YoutifyBotContext>();
builder.Services.AddSingleton<CliBot>();

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