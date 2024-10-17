namespace VoiceMeterBot;

internal static class Program
{
    private static async Task Main()
    {
        var bot = new Bot(BotToken, GuildId, VoiceChannelId);
        await bot.Connect();
        await Task.Delay(-1);
    }
}