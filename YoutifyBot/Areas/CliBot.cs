using TL;
using WTelegram;

namespace YoutifyBot.Areas;

public class CliBot
{
    static IConfigurationSection configurationSections;
    static Client clientBot;
    public CliBot()
    {
        configurationSections = new ConfigurationBuilder().AddJsonFile("logininformations.json").Build().GetSection("profile");
        
        clientBot = new Client(int.Parse(configurationSections["api_id"]), configurationSections["api_hash"], AppContext.BaseDirectory + "Sessions\\WTelegram.session");
        DoLoginAsync(configurationSections["phone_number"]);
        Helpers.Log = (lvl, str) => { };
    }

    public async Task LoginAsync(string code)
    {
        await DoLoginAsync(code);
    }

    public async Task<int> SendAndGetMediaMessageIdAsync(Stream stream, bool isMovie)
    {
        string type = isMovie ? ".mp4" : ".mp3";
        string mime_type = isMovie ? "video/mp4" : "audio/mpeg";
        var file = await clientBot.UploadFileAsync(new Helpers.IndirectStream(stream), $"Youtify.{type}");
        var sentMessage = await clientBot.SendMessageAsync(new InputPeerChannel(1864845042, 1200396395369489557), "", new InputMediaUploadedDocument
        {
            file = file,
            mime_type = mime_type,
            attributes = new[]
            {
                new DocumentAttributeVideo
                {
                    flags = DocumentAttributeVideo.Flags.supports_streaming
                }
            }
        });
        return sentMessage.ID;
    }

    public async Task DoLoginAsync(string loginInfo) // (add this method to your code)
    {
        await clientBot.Login(loginInfo);
    }

    ~CliBot()
    {
        clientBot.Dispose();
    }
}
