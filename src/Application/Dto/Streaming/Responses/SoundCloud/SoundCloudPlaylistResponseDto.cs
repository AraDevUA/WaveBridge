using System.Text.Json.Serialization;

namespace Application.Dto.Streaming.Responses.SoundCloud;

public record SoundCloudPlaylistResponseDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }

    [JsonPropertyName("artwork_url")]
    public string? ArtworkUrl { get; init; }

    [JsonPropertyName("permalink_url")]
    public string? PermalinkUrl { get; init; }

    public List<SoundCloudTrackDto> Tracks { get; init; } = new();
}
