using Microsoft.EntityFrameworkCore;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<YoutifyBotContext>();
//builder.Services.AddDbContext<YoutifyBotContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("YoutifyConnection")));

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "YoutifyBot",
    areaName: "YoutifyBot",
    pattern: "YoutifyBot/{controller}/{action}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
