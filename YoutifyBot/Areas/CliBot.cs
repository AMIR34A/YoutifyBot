using TL;
using WTelegram;

namespace YoutifyBot.Areas;

public class CliBot
{
    static IConfigurationSection configurationSections;
    Client clientBot;
    public CliBot(Client clientBot)
    {
        new ConfigurationManager().AddJsonFile("logininformations.json").Build().Reload();
        configurationSections = new ConfigurationBuilder().AddJsonFile("logininformations.json").Build().GetSection("profile");
        this.clientBot = clientBot;
        DoLogin(configurationSections["phone_number"]);
    }

    public async Task LoginAsync()
    {
        await clientBot.LoginUserIfNeeded();
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

    async Task DoLogin(string loginInfo) // (add this method to your code)
    {
        while (clientBot.User == null)
            switch (await clientBot.Login(loginInfo)) // returns which config is needed to continue login
            {
                case "verification_code": Console.Write("Code: "); loginInfo = Console.ReadLine(); break;
                case "password": loginInfo = configurationSections["password"]; break;
                default: loginInfo = null; break;
            }
    }
}
