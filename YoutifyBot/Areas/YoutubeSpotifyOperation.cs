using Telegram.Bot.Types.ReplyMarkups;
using YoutubeExplode;

namespace YoutifyBot.Areas;

public class YoutubeSpotifyOperation
{
    static YoutubeClient youtubeClient;
    public YoutubeSpotifyOperation()
    {
        youtubeClient = new YoutubeClient();
    }

    public async Task<IEnumerable<IEnumerable<InlineKeyboardButton>>> GetAllVideoQualities(string url)
    {
        List<IEnumerable<InlineKeyboardButton>> qualities = new List<IEnumerable<InlineKeyboardButton>>();

        var manifests = await youtubeClient.Videos.Streams.GetManifestAsync(url);

        var audioes = manifests.GetAudioOnlyStreams().OrderBy(audio => audio.Size.MegaBytes);
        var videos = manifests.GetMuxedStreams();


        foreach (var video in videos)
            qualities.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"🎥 | {video.Size.MegaBytes:F2}Mb", $"{video.Size.MegaBytes}") });

        foreach (var audio in audioes)
            qualities.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"🎧 | {audio.Size.MegaBytes:F2}Mb", $"{audio.Size.MegaBytes}") });

        return qualities;
    }
}
