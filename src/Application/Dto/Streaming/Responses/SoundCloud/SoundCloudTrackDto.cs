using System.Text.Json.Serialization;

namespace Application.Dto.Streaming.Responses.SoundCloud;

public record SoundCloudTrackDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public SoundCloudUserDto? User { get; init; }

    [JsonPropertyName("publisher_metadata")]
    public SoundCloudPublisherMetadataDto? PublisherMetadata { get; init; }

    [JsonPropertyName("artwork_url")]
    public string? ArtworkUrl { get; init; }
}
