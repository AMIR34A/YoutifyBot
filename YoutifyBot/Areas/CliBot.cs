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
    }

    public async Task LoginAsync()
    {
        using Client client = new Client(Config);
        await client.LoginUserIfNeeded();
    }

    public async Task<int> SendAndGetMediaMessageIdAsync(Stream stream, bool isMovie)
    {
        await clientBot.LoginUserIfNeeded();
        string type = isMovie ? "Youtify.mp4" : "Youtify.mp3";
        string mime_type = isMovie ? "video/mp4" : "audio/mpeg";
        var file = await clientBot.UploadFileAsync(new Helpers.IndirectStream(stream), $"Youtify.{type}");
        var sentMessage = await clientBot.SendMessageAsync(new InputPeerChannel(1864845042, 1238299960073467510), "", new InputMediaUploadedDocument
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

    public static string Config(string what)
    {
        new ConfigurationManager().AddJsonFile("logininformations.json").Build().Reload();
        configurationSections = new ConfigurationBuilder().AddJsonFile("logininformations.json").Build().GetSection("profile");

        switch (what)
        {
            case "api_id": return configurationSections["api_id"];
            case "api_hash": return configurationSections["api_hash"];
            case "phone_number": return configurationSections["phone_number"];
            case "verification_code": return configurationSections["verification_code"];
            //case "first_name": return configurationSections["first_name"];
            //case "last_name": return configurationSections["last_name"];
            default: return null;
        }
    }

}
