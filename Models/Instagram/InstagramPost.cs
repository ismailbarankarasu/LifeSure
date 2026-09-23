using System.Text.Json.Serialization;

namespace LifeSure.Models.Instagram;

public sealed class InstagramPost
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("displayUrl")]
    public string DisplayUrl { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }
}