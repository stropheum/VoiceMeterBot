using Newtonsoft.Json;

namespace VoiceMeterBot;

[Serializable]
public class MessageLogModel
{
    [JsonProperty] public string? Name { get; set; }
    [JsonProperty] public string? Payload { get; set; }
}