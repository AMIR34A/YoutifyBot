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
    public BotController()
    {
        _botClient = new TelegramBotClient("6398637615:AAFqcxLt-HuY16lOPybbZYhlx1jVF6iK54Y");

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

    private static async Task HandleUpdateAsyns(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        BotResponse botResponse = new BotResponse();
        if (update.Type == UpdateType.Message && update.Message.Chat.Type == ChatType.Private)
            botResponse.ResponseToText(_botClient, update);
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
