using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;
using User = YoutifyBot.Models.User;

namespace YoutifyBot.Areas.YoutifyBot;

public class BotResponse
{
    public async Task ResponseToText(TelegramBotClient botClient, Update update)
    {
        StringBuilder stringBuilder = new StringBuilder();

        if (update.Message.Text.Equals("/start"))
        {
            long chatId = update.Message.Chat.Id;
            using (IUnitOfWork unitOfWork = new UnitOfWork(new YoutifyBotContext()))
            {
                User user = await unitOfWork.Repository<User>().FindByChatId(chatId);
                if (user is null)
                {
                    User newUser = new User
                    {
                        ChatId = chatId,
                        FirstName = update.Message.Chat.FirstName,
                        LastName = update.Message.Chat.LastName,
                        Username = update.Message.Chat.Username
                    };
                    await unitOfWork.Repository<User>().CreateAsync(newUser);
                    await unitOfWork.SaveAsync();
                }
            }
            stringBuilder.AppendLine("Hi my friend👋");
            stringBuilder.AppendLine("•You can download from youtube and spotify;");
            stringBuilder.AppendLine("•Also you can use the <b>Menu</b> button for more informations");
            stringBuilder.AppendLine("<b>⚙️I'm ready, just send me the link😃</b>");
            await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), null, ParseMode.Html);
        }
        if (update.Message.Text.StartsWith("https://www.youtube.com/watch?v="))
        {

        }
    }
}
