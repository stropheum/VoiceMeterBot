using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using Newtonsoft.Json;

namespace VoiceMeterBot;

public class Bot
{
    private GatewayClient _client;
    private VoiceClient _voiceClient = null!;
    private HashSet<User> _users = new();
    private User _user = null!;
    private ulong _guildId;
    private ulong _voiceChannelId;

    public Bot(string botToken, ulong guildId, ulong voiceChannelId)
    {
        _client = new GatewayClient(new Token(TokenType.Bot, botToken), new GatewayClientConfiguration()
        {
            Intents = GatewayIntents.All
        });
        _guildId = guildId;
        _voiceChannelId = voiceChannelId;
    }

    public async Task Connect()
    {
        _client.Log += OnLog;
        _client.Ready += OnReady;
        _client.VoiceStateUpdate += OnVoiceStateUpdate;
        await _client.StartAsync();
    }

    private async ValueTask OnVoiceStateUpdate(VoiceState arg)
    {
        LogOutputMessage("VoiceStateUpdate", JsonConvert.SerializeObject(arg));

        // Don't handle events for when the bot joins
        if (arg.UserId == _voiceClient.UserId) return;

        if (arg.ChannelId == _voiceChannelId)
            await OnVoiceChannelJoined(arg);
        else
            await OnVoiceChannelLeft(arg);
        await Task.CompletedTask;
    }

    private ValueTask OnVoiceChannelJoined(VoiceState arg)
    {
        LogOutputMessage("VoiceChannelJoined", JsonConvert.SerializeObject(arg.User));
        if (arg.User != null) _users.Add(arg.User);

        return ValueTask.CompletedTask;
    }

    private ValueTask OnVoiceChannelLeft(VoiceState arg)
    {
        LogOutputMessage("VoiceChannelLeft", JsonConvert.SerializeObject(arg.User));
        if (arg.User != null) _users.Remove(arg.User);

        return ValueTask.CompletedTask;
    }

    private async ValueTask OnReady(ReadyEventArgs arg)
    {
        LogOutputMessage("OnReady", JsonConvert.SerializeObject(arg));
        _user = arg.User;

        _voiceClient = await _client.JoinVoiceChannelAsync(
            _user.Id,
            _guildId,
            _voiceChannelId,
            new VoiceClientConfiguration { RedirectInputStreams = true });
        _voiceClient.Ready += VoiceClientOnReady;
        _voiceClient.VoiceReceive += OnVoiceReceive;
        await _voiceClient.StartAsync();
    }

    private ValueTask VoiceClientOnReady()
    {
        LogOutputMessage("VoiceClientOnReady", string.Empty);
        return ValueTask.CompletedTask;
    }

    private async ValueTask OnVoiceReceive(VoiceReceiveEventArgs arg)
    {
        var user = _users.FirstOrDefault(x => x.Id == arg.UserId) ?? await _client.Rest.GetUserAsync(arg.UserId);
        _users.Add(user);
        var receivedArgs = new VoiceActivityEventArgs
        {
            TimeStamp = DateTime.Now,
            User = user,
            Ssrc = arg.Ssrc,
            UserId = arg.UserId,
            Frame = arg.Frame.ToArray()
        };
        LogOutputMessage("VoiceReceive", JsonConvert.SerializeObject(receivedArgs));
    }

    private static ValueTask OnLog(LogMessage message)
    {
        Console.WriteLine(message);
        return default;
    }

    private static void LogOutputMessage(string messageName, string payload)
    {
        var model = new MessageLogModel()
        {
            Name = messageName,
            Payload = payload
        };
        Console.WriteLine(JsonConvert.SerializeObject(model));
    }
}