using NetCord;
using NetCord.Gateway.Voice;
using Newtonsoft.Json;

namespace VoiceMeterBot;

[Serializable]
public class VoiceActivityEventArgs
{
    [JsonProperty] public DateTime TimeStamp { get; set; }
    [JsonProperty] public User? User { get; set; }
    [JsonProperty] public uint Ssrc { get; set; }
    [JsonProperty] public ulong UserId { get; set; }
    [JsonProperty] public byte[]? Frame { get; set; }
}