using System.Net;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TL;
using WTelegram;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;
using Update = Telegram.Bot.Types.Update;
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
        YoutubeSpotifyOperation youtubeSpotifyOperation = new YoutubeSpotifyOperation();

        var user = await botClient.GetChatMemberAsync("@YoutifyNews", update.CallbackQuery.From.Id);
        if (user.Status == ChatMemberStatus.Left)
        {
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "For dwonloading you have to join in the @YoutifyNews \n Link in the bio", true);
            return;
        }

        string url = await youtubeSpotifyOperation.GetDonwloadUrlAsync(update.CallbackQuery);

        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        var client = new HttpClient();

        using (var stream = await client.GetStreamAsync(url))
        {
            Client clientBot = new Client(Config);
            await clientBot.LoginUserIfNeeded();

            try
            {
                var contentLnegth = await client.GetAsync(url);
                var file = await clientBot.UploadFileAsync(new Helpers.IndirectStream(stream) { ContentLength = contentLnegth.Content.Headers.ContentLength }, "test.mp4");
                await clientBot.SendMediaAsync(InputPeer.Self, "test", file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }





            // await botClient.SendVideoAsync(update.CallbackQuery.From.Id, new InputFileStream(stream));
        }





    }
    static string Config(string what)
    {
        switch (what)
        {
            case "api_id": return "10028138";
            case "api_hash": return "db88d1744ab8e4ea8d213b5b960a5423";
            case "phone_number": return "+989152663439";
            case "verification_code": return "";
            //case "first_name": return configurationSections["first_name"];
            //case "last_name": return configurationSections["last_name"];
            default: return null;
        }
    }
}
