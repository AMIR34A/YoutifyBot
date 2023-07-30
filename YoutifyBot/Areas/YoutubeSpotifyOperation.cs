using Telegram.Bot.Types;
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
            qualities.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"🎥 | {Math.Round(video.Size.MegaBytes)}Mb", $"YoutubeMovie|{Math.Round(video.Size.MegaBytes)}") });

        foreach (var audio in audioes)
            qualities.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"🎧 | {Math.Round(audio.Size.MegaBytes)}Mb", $"YoutubeMusic|{Math.Round(audio.Size.MegaBytes)}") });

        return qualities;
    }

    public async Task<Stream> DownloadMediaAsync(string url, double size, bool isMovie)
    {
        var manifests = await youtubeClient.Videos.Streams.GetManifestAsync(url);
        dynamic streamInfo = isMovie ? manifests.GetMuxedStreams().Where(audio => Math.Round(audio.Size.MegaBytes) == size).First() :
                                       manifests.GetAudioOnlyStreams().Where(audio => Math.Round(audio.Size.MegaBytes) == size).First();
        var stream = await youtubeClient.Videos.Streams.GetAsync(streamInfo);
        return stream;
    }
}
