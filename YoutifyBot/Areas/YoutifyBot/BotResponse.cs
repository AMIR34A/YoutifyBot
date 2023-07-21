using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;
using User = YoutifyBot.Models.User;

namespace YoutifyBot.Areas.YoutifyBot;

public class BotResponse
{
    public async Task ResponseToText(TelegramBotClient botClient, Update update)
    {
        if (update.Message.Type != MessageType.Text)
            return;

        StringBuilder stringBuilder = new StringBuilder();
        long chatId = update.Message.Chat.Id;
        string text = update.Message.Text;

        if (text.Equals("/start"))
        {
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

        else if (text.Equals("/help"))
        {
            stringBuilder.AppendLine("You just need to send video or music link to me");
            stringBuilder.AppendLine("<b>Your link for downloading video and music must have this formats :</b>");
            stringBuilder.AppendLine("🎞https://www.youtube.com/... | https://youtu.be/...");
            stringBuilder.AppendLine("🎵https://open.spotify.com/track/...");
            await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), null, ParseMode.Html, null, true);
        }

        else if (text.StartsWith("https://www.youtube.com/") || text.StartsWith("https://youtu.be/"))
        {
            YoutubeSpotifyOperation youtubeSpotifyOperation = new YoutubeSpotifyOperation();

            if (text.StartsWith("https://www.youtube.com/live"))
                text = text.Replace("live/", "watch?v=").Replace("?feature=share", "");

            var qualities = await youtubeSpotifyOperation.GetAllVideoQualities(text);

            stringBuilder.AppendLine($"🔗<a href=\"{text}\">Your video link</a>");
            stringBuilder.AppendLine("✔️<b>Select the format and quality :</b>");

            await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), null,
                  ParseMode.Html, null, true, false, null, update.Message.MessageId, null, new InlineKeyboardMarkup(qualities));
        }
    }

    public async Task ResponseToCallBackQuery(TelegramBotClient botClient, Update update)
    {
        if (update.CallbackQuery.Data is null)
            return;
        var user = await botClient.GetChatMemberAsync("YoutifyNews", update.CallbackQuery.From.Id);
        if (user.Status == ChatMemberStatus.Left)
            await botClient.EditMessageTextAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "For dwonloading you have to join in the <b>@YoutifyNews</b>",
                ParseMode.Html);
    }
}
