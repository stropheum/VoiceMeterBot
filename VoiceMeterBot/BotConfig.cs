namespace VoiceMeterBot;

public class BotConfig
{
    public string BotToken { get; set; } = string.Empty;
    public ulong VoiceChannelId { get; set; }
    public ulong GuildId { get; set; }
}