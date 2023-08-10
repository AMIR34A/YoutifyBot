using SpotifyAPI.Web;
using Telegram.Bot.Types.ReplyMarkups;
using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;

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
        var videos = manifests.GetMuxedStreams().OrderBy(video => video.Size.MegaBytes);


        foreach (var video in videos)
            qualities.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"🎥 | {video.Size.MegaBytes.ToString("F2")}MB", $"YoutubeMovie|{video.Size.MegaBytes.ToString("F2")}") });

        foreach (var audio in audioes)
            qualities.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"🎧 | {audio.Size.MegaBytes.ToString("F2")}MB", $"YoutubeMusic|{audio.Size.MegaBytes.ToString("F2")}") });

        return qualities;
    }

    public async Task<Stream> DownloadMediaAsync(string url, double? size, bool isMovie)
    {
        var manifests = await youtubeClient.Videos.Streams.GetManifestAsync(url);

        if (!size.HasValue)
        {
            var musicStreamInfo = manifests.GetAudioOnlyStreams().TryGetWithHighestBitrate();
            Stream musicStream = await youtubeClient.Videos.Streams.GetAsync(musicStreamInfo);
            return musicStream;
        }

        dynamic streamInfo = isMovie ? manifests.GetMuxedStreams().MinBy(audio => Math.Abs(audio.Size.MegaBytes - size.Value)) :
                   manifests.GetAudioOnlyStreams().MinBy(audio => Math.Abs(audio.Size.MegaBytes - size.Value));
        var stream = await youtubeClient.Videos.Streams.GetAsync(streamInfo);
        return stream;
    }

    public async Task<Stream> DownloadMediaBySearchAsync(string searchQuery)
    {
        string[] musicDetails = searchQuery.Split('|');
        await foreach (var item in youtubeClient.Search.GetResultsAsync($"song {musicDetails[0]} by {musicDetails[1]}"))
        {
            switch (item)
            {
                case VideoSearchResult video:
                    var stream = await DownloadMediaAsync(video.Url, 20, false);
                    return stream;
            }
        }
        return await Task.FromResult<Stream>(null);
    }

    public async Task<string> GetMusicInfoAsync(string url)
    {
        var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator("76a349a2073c402694e21bff6aefffca", "b857ae9c44f44bc7a0c3d0f7cde999cf"));
        SpotifyClient spotifyClient = new SpotifyClient(config);
        string newUrl = url.Split('?')[0].Replace("https://open.spotify.com/track/", "");
        var track = await spotifyClient.Tracks.Get(newUrl);
        return $"{track.Name}|{track.Artists.First().Name}";
    }
}
