using System.Text.Json.Serialization;

namespace Application.Dto.Streaming.Responses.SoundCloud;

public record SoundCloudLikedTracksResponseDto
{
    [JsonPropertyName("collection")]
    public List<SoundCloudTrackDto> Collection { get; init; } = [];

    [JsonPropertyName("total_count")]
    public int TotalCount { get; init; }

    [JsonPropertyName("next_href")]
    public string? NextHref { get; init; }
}