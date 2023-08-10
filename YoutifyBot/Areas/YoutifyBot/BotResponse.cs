using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using YoutifyBot.Models;
using YoutifyBot.Models.Repository;
using Update = Telegram.Bot.Types.Update;
using User = YoutifyBot.Models.User;

namespace YoutifyBot.Areas.YoutifyBot;

public class BotResponse
{
    CliBot cliBot;
    public BotResponse(CliBot cliBot)
    {
        this.cliBot = cliBot;
    }

    public async Task ResponseToText(TelegramBotClient botClient, Update update)
    {
        if (update.Message.Type != MessageType.Text)
            return;

        StringBuilder stringBuilder = new StringBuilder();
        long chatId = update.Message.Chat.Id;
        string text = update.Message.Text;
        YoutubeSpotifyOperation youtubeSpotifyOperation = new YoutubeSpotifyOperation();

        switch (text)
        {
            case string when text.Contains("/start"):
                using (IUnitOfWork unitOfWork = new UnitOfWork(new YoutifyBotContext()))
                {
                    Rule rule = await unitOfWork.Repository<Rule>().GetFirstAsync();
                    User user = await unitOfWork.Repository<User>().FindByChatId(chatId);

                    if (text.Contains("invite"))
                    {
                        long invitedByChatId = long.Parse(text.Split('_')[1]);
                        bool isExist = await unitOfWork.Repository<User>().AynAsync(update.Message.Chat.Id);
                        if (!isExist)
                        {
                            var invitedByuser = await unitOfWork.Repository<User>().FindByChatId(invitedByChatId);
                            if (invitedByuser.MaximumDownloadSize < rule.MaximumDownloadSize)
                                invitedByuser.MaximumDownloadSize += rule.AmountRewardInviting;
                            await unitOfWork.SaveAsync();
                            stringBuilder.AppendLine("One user was invited by you🎉");
                            stringBuilder.AppendLine("The maximum size of video you can download was updated.");
                            stringBuilder.AppendLine("<b>You can see more detail in your profile</b>");
                            await botClient.SendTextMessageAsync(invitedByChatId, stringBuilder.ToString(), null, ParseMode.Html);
                            stringBuilder.Clear();
                        }
                        else
                            await botClient.SendTextMessageAsync(invitedByChatId, "<b>⚠️The user already exist</b>", parseMode: ParseMode.Html);
                    }

                    if (user is null)
                    {
                        User newUser = new User
                        {
                            ChatId = chatId,
                            FirstName = update.Message.Chat.FirstName,
                            LastName = update.Message.Chat.LastName,
                            Username = update.Message.Chat.Username,
                            MaximumDownloadSize = rule.BaseDownloadSize,
                            TotalDonwload = 0,
                            UserRole = Role.Member
                        };
                        await unitOfWork.Repository<User>().CreateAsync(newUser);
                    }
                    await unitOfWork.SaveAsync();
                }

                stringBuilder.AppendLine("Hi my friend👋");
                stringBuilder.AppendLine("•You can download from youtube and spotify;");
                stringBuilder.AppendLine("•Also you can use the <b>Menu</b> button for more informations");
                stringBuilder.AppendLine("<b>I'm ready, just send me the link😃</b>");
                await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), parseMode: ParseMode.Html);
                break;

            case string when text.StartsWith("https://www.youtube.com/"):
            case string when text.StartsWith("https://youtu.be/"):
            case string when text.StartsWith("https://youtube.com/"):

                string convertedUrl = ConvertUrlToId(text);
                var qualities = await youtubeSpotifyOperation.GetAllVideoQualities(convertedUrl);

                stringBuilder.AppendLine($"🔗<a href=\"{convertedUrl}\">Your video link</a>");
                stringBuilder.AppendLine("✔️<b>Select the format and quality :</b>");

                await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), parseMode: ParseMode.Html,
                    disableWebPagePreview: true, replyToMessageId: update.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(qualities));
                break;


            case string when text.StartsWith("https://open.spotify.com/track/"):
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "<pre>⚙️Processing is started...</pre>",
                              parseMode: ParseMode.Html, replyToMessageId: update.Message.MessageId);
                string searchQuery = await youtubeSpotifyOperation.GetMusicInfoAsync(text);
                var stream = await youtubeSpotifyOperation.DownloadMediaBySearchAsync(searchQuery);
                string[] musicDetails = searchQuery.Split('|');
                await botClient.SendAudioAsync(update.Message.Chat.Id, new InputFileStream(stream), title: musicDetails[0], performer: musicDetails[1]);
                break;

            case "/help":
                stringBuilder.AppendLine("You just need to send video or music link to me.");
                stringBuilder.AppendLine("<b>Your link for downloading video and music must have this formats :</b>");
                stringBuilder.AppendLine("<pre>🎞Youtube :</pre>");
                stringBuilder.AppendLine("▪️https://www.youtube.com/...");
                stringBuilder.AppendLine("▪️https://youtu.be/...");
                stringBuilder.AppendLine("<pre>🎵Spotify</pre>");
                stringBuilder.AppendLine("▪️https://open.spotify.com/track/...");
                await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), parseMode: ParseMode.Html, disableWebPagePreview: true);
                break;

            case "/upgrade":
                using (IUnitOfWork unitOfWork = new UnitOfWork(new YoutifyBotContext()))
                {
                    Rule rule = await unitOfWork.Repository<Rule>().GetFirstAsync();
                    User user = await unitOfWork.Repository<User>().FindByChatId(chatId);

                    stringBuilder.AppendLine("<b>📈You can invite your friends to the bot, and update your maximum size download.</b>");
                    stringBuilder.AppendLine($"<pre>•You gain {rule.AmountRewardInviting}MB for each user who you invited.</pre>");
                    stringBuilder.AppendLine($"<pre>•Your maximum size download : {user.MaximumDownloadSize}MB.</pre>");
                    stringBuilder.AppendLine($"<pre>•Now you can increase it to {rule.MaximumDownloadSize}MB.</pre>");
                    stringBuilder.AppendLine("<b>🔐Use below button for your inviting link :</b>");
                    await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), parseMode: ParseMode.Html,
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🔗Inviting Link", "InvitingLink")));
                }
                break;
        }
    }

    public async Task ResponseToCallBackQuery(TelegramBotClient botClient, Update update)
    {
        if (!Enum.Equals(update.Type, UpdateType.CallbackQuery))
            return;

        //var userStatus = await botClient.GetChatMemberAsync("@YoutifyNews", update.CallbackQuery.From.Id);
        //if (userStatus.Status == ChatMemberStatus.Left)
        //{
        //    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "For dwonloading you have to join in the @YoutifyNews \n Link in the bio", true);
        //    return;
        //}

        YoutubeSpotifyOperation youtubeSpotifyOperation = new YoutubeSpotifyOperation();

        var data = update.CallbackQuery.Data;
        StringBuilder stringBuilder = new StringBuilder();

        if (data.StartsWith("Youtube"))
        {
            string url = update.CallbackQuery.Message.Entities[0].Url;
            double size = double.Parse(data.Split('|')[1]);

            using (IUnitOfWork unitOfWork = new UnitOfWork(new YoutifyBotContext()))
            {
                User user = await unitOfWork.Repository<User>().FindByChatId(update.CallbackQuery.From.Id);

                if (user.MaximumDownloadSize < size)
                {
                    stringBuilder.AppendLine($"‼️<b>You can't download video/music file with volume bigger than {user.MaximumDownloadSize}MB</b>");
                    stringBuilder.AppendLine("<b>💢Use /upgrae command for more infomations</b>");
                    await botClient.EditMessageTextAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, stringBuilder.ToString(), ParseMode.Html);
                    return;
                }

                await botClient.EditMessageTextAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "<pre>⚙️Processing is started...</pre>",
                            ParseMode.Html);
                if (data.StartsWith("YoutubeMovie"))
                {
                    var stream = await youtubeSpotifyOperation.DownloadMediaAsync(url, size, true);

                    if (size > 50)
                    {
                        int mediaMessageId = await cliBot.SendAndGetMediaMessageIdAsync(stream, true);
                        await botClient.SendChatActionAsync(update.CallbackQuery.From.Id, ChatAction.UploadVideo);
                        await botClient.SendVideoAsync(update.CallbackQuery.From.Id, new InputFileId($"https://t.me/YoutifyArchive/{mediaMessageId}"));
                    }
                    else
                    {
                        await botClient.SendChatActionAsync(update.CallbackQuery.From.Id, ChatAction.UploadVideo);
                        await botClient.SendVideoAsync(update.CallbackQuery.From.Id, new InputFileStream(stream));
                    }
                }
                else
                {
                    var stream = await youtubeSpotifyOperation.DownloadMediaAsync(url, size, false);

                    if (size > 50)
                    {
                        int mediaMessageId = await cliBot.SendAndGetMediaMessageIdAsync(stream, false);
                        await botClient.SendChatActionAsync(update.CallbackQuery.From.Id, ChatAction.UploadVoice);
                        await botClient.SendAudioAsync(update.CallbackQuery.From.Id, new InputFileId($"https://t.me/YoutifyArchive/{mediaMessageId}"));
                    }
                    else
                    {
                        await botClient.SendChatActionAsync(update.CallbackQuery.From.Id, ChatAction.RecordVideo);
                        await botClient.SendAudioAsync(update.CallbackQuery.From.Id, new InputFileStream(stream));
                    }
                }
                await unitOfWork.SaveAsync();
            }
        }
        else if (data.Equals("InvitingLink"))
        {
            await botClient.DeleteMessageAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId);
            stringBuilder.AppendLine("<pre>🖥You can download Youtube video and audio</pre>");
            stringBuilder.AppendLine("<pre>🎵You can download Spotify music</pre>");
            stringBuilder.AppendLine("<b>🚀Just start the bot : </b>");
            stringBuilder.AppendLine($"https://t.me/YoutifyBot?start=invite_{update.CallbackQuery.From.Id}");
            await botClient.SendPhotoAsync(update.CallbackQuery.From.Id, new InputFileId("https://t.me/YoutifyNews/16"), caption: stringBuilder.ToString(), parseMode: ParseMode.Html);
        }
    }

    public string ConvertUrlToId(string url)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (url.StartsWith("https://www.youtube.com/watch"))
            stringBuilder.Append(url);
        else if (url.StartsWith("https://youtu.be/"))
            stringBuilder.Append(url);
        else if (url.StartsWith("https://www.youtube.com/live/"))
            stringBuilder.Append($"https://www.youtube.com/watch?v={url.Remove(0, 29).Replace("?feature=share", "")}");
        else if (url.StartsWith("https://youtube.com/shorts/"))
            stringBuilder.Append($"https://www.youtube.com/watch?v={url.Remove(0, 27).Replace("?feature=share", "")}");
        else if (url.StartsWith("https://www.youtube.com/shorts/"))
            stringBuilder.Append($"https://www.youtube.com/watch?v={url.Remove(0, 31).Replace("?feature=share", "")}");
        return stringBuilder.ToString();
    }
}
