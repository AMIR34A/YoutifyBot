using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace YoutifyBot.Areas.YoutifyBot.Controllers;

[Area("YoutifyBot")]
public class BotController : Controller
{
    static TelegramBotClient _botClient;
    static CliBot cliBot;
    public BotController(CliBot cli) => cliBot = cli;

    static BotController()
    {
        _botClient = new TelegramBotClient("6398637615:AAGXLoAqIrt2Rp64j_thLqJ1yMWxfIrowos");

        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        _botClient.StartReceiving(HandleUpdateAsyns, HandleErrorAsync, receiverOptions);
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Login()
    {
        //await cliBot.LoginAsync();
        return View("Index");
    }
    private static async Task HandleUpdateAsyns(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        BotResponse botResponse = new BotResponse(cliBot);
        try
        {
            if (update.Type == UpdateType.Message && update.Message.Chat.Type == ChatType.Private)
                botResponse.ResponseToText(_botClient, update);
            else if (update.Type == UpdateType.CallbackQuery)
                botResponse.ResponseToCallBackQuery(_botClient, update);
        }
        catch { }
    }

    private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
    }
}
