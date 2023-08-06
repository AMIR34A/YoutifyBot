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
                            if (invitedByuser.TotalDownloadVolume < rule.MaximumDownloadVolume)
                                invitedByuser.TotalDownloadVolume += 100;
                            await unitOfWork.SaveAsync();
                            stringBuilder.AppendLine("One user was invited by you🎉");
                            stringBuilder.AppendLine("The maximum size of video you can download was updated.");
                            stringBuilder.AppendLine("<b>You can see more detail in your profile</b>");
                            await botClient.SendTextMessageAsync(invitedByChatId, stringBuilder.ToString(), null, ParseMode.Html,
                                replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🖥Profile", "Profile")));
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
                            TotalDownloadVolume = rule.BaseDownloadVolume,
                            TotalCountDownload = rule.BaseCountDownload,
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


            case string when text.Equals("/help"):
                stringBuilder.AppendLine("You just need to send video or music link to me.");
                stringBuilder.AppendLine("<b>Your link for downloading video and music must have this formats :</b>");
                stringBuilder.AppendLine("<pre>🎞Youtube :</pre>");
                stringBuilder.AppendLine("▪️https://www.youtube.com/...");
                stringBuilder.AppendLine("▪️https://youtu.be/...");
                stringBuilder.AppendLine("<pre>🎵Spotify</pre>");
                stringBuilder.AppendLine("▪️https://open.spotify.com/track/...");
                await botClient.SendTextMessageAsync(chatId, stringBuilder.ToString(), parseMode: ParseMode.Html, disableWebPagePreview: true);
                break;


            case string when text.StartsWith("https://www.youtube.com/"):
            case string when text.StartsWith("https://youtu.be/"):
                if (text.StartsWith("https://www.youtube.com/live"))
                    text = text.Replace("live/", "watch?v=").Replace("?feature=share", "");

                var qualities = await youtubeSpotifyOperation.GetAllVideoQualities(text);

                stringBuilder.AppendLine($"🔗<a href=\"{text}\">Your video link</a>");
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
        }
    }

    public async Task ResponseToCallBackQuery(TelegramBotClient botClient, Update update)
    {
        if (!Enum.Equals(update.Type, UpdateType.CallbackQuery))
            return;

        var userStatus = await botClient.GetChatMemberAsync("@YoutifyNews", update.CallbackQuery.From.Id);
        if (userStatus.Status == ChatMemberStatus.Left)
        {
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "For dwonloading you have to join in the @YoutifyNews \n Link in the bio", true);
            return;
        }

        YoutubeSpotifyOperation youtubeSpotifyOperation = new YoutubeSpotifyOperation();

        var data = update.CallbackQuery.Data;

        try
        {
            if (data.StartsWith("Youtube"))
            {
                string url = update.CallbackQuery.Message.Entities[0].Url;
                double size = double.Parse(data.Split('|')[1]);

                StringBuilder stringBuilder = new StringBuilder();
                using (IUnitOfWork unitOfWork = new UnitOfWork(new YoutifyBotContext()))
                {
                    User user = await unitOfWork.Repository<User>().FindByChatId(update.CallbackQuery.From.Id);

                    if (user.TotalDownloadVolume < size)
                    {
                        stringBuilder.AppendLine($"‼️<b>You can't download video/music file with volume bigger than {user.TotalDownloadVolume}MB</b>");
                        await botClient.EditMessageTextAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, stringBuilder.ToString(), ParseMode.Html);
                        return;
                    }
                    else if (user.TotalDownloadVolume / 2 < size && user.TotalCountDownload == 0)
                    {
                        stringBuilder.AppendLine($"‼️<b>You can download video/music file with volume bigger than a half of your total download volume({user.TotalDownloadVolume / 2}MB), twice a day</b>");
                        await botClient.EditMessageTextAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, stringBuilder.ToString(),
                                ParseMode.Html);
                        return;
                    }
                    else if (user.TotalDownloadVolume / 2 < size && user.TotalCountDownload > 0)
                        user.TotalCountDownload--;

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
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
