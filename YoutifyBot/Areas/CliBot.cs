using TL;
using WTelegram;

namespace YoutifyBot.Areas;

public class CliBot
{
    static IConfigurationSection configurationSections;

    public CliBot()
    {
        new ConfigurationManager().AddJsonFile("logininformations.json").Build().Reload();
        configurationSections = new ConfigurationBuilder().AddJsonFile("logininformations.json").Build().GetSection("profile");
    }

    public async Task LoginAsync()
    {
        using Client client = new Client(Config);
        await client.LoginUserIfNeeded();
    }

    public async Task<int> SendAndGetMediaMessageIdAsync(string url)
    {

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("connection", "keep-alive");
            client.DefaultRequestHeaders.ConnectionClose = false;

            using (var request = await client.GetAsync(url))
            {
                using var stream = await request.Content.ReadAsStreamAsync();

                using Client clientBot = new Client(Config);
                await clientBot.LoginUserIfNeeded();

                var file = await clientBot.UploadFileAsync(new Helpers.IndirectStream(stream), "Youtify.mp4");
                var sentMessage = await clientBot.SendMessageAsync(new InputPeerChannel(1864845042, 1238299960073467510), "", new InputMediaUploadedDocument
                {
                    file = file,
                    mime_type = "video/mp4",
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
        }
    }

    static string Config(string what)
    {
        switch (what)
        {
            case "api_id": return configurationSections["api_id"];
            case "api_hash": return configurationSections["api_hash"];
            case "phone_number": return configurationSections["phone_number"];
            case "verification_code":
                new ConfigurationManager().AddJsonFile("logininformations.json").Build().Reload();
                return configurationSections["verification_code"];
            //case "first_name": return configurationSections["first_name"];
            //case "last_name": return configurationSections["last_name"];
            default: return null;
        }
    }

}
