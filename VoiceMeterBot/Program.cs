using Newtonsoft.Json;

namespace VoiceMeterBot;

//NOTE: Requires libsodium.dll to be in the same path as the executable. Underlying API relies on libsodium
internal static class Program
{
    private static async Task Main()
    {
        try
        {
            var streamReader = new StreamReader("config.json");
            var json = await streamReader.ReadToEndAsync();
            var config = JsonConvert.DeserializeObject<BotConfig>(json);
            if (config == null)
            {
                Console.WriteLine("config == null. Make sure config.json exists and is formatted correctly. Exiting...");
            }

            var bot = new Bot(config);
            await bot.Connect();
            await Task.Delay(-1);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("config.json required to run at executable directory: " + e);
        }
        catch (JsonSerializationException e)
        {
            Console.WriteLine("Failed to parse config.json. Make sure config.json exists and contains data in format:\n" +
            "{\n" +
                "\"BotToken\": \"teststring1234\"\n" +
                "\"GuildID\": 123123123123123\n" +
                "\"VoiceChannelID\": 123123123123123\n" +
            "}");
        }
    }
}