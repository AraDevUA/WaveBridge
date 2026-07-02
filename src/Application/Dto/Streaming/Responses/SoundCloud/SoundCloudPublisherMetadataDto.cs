using System.Text.Json.Serialization;

namespace Application.Dto.Streaming.Responses.SoundCloud;

public record SoundCloudPublisherMetadataDto
{
    [JsonPropertyName("album_title")]
    public string? AlbumTitle { get; init; }

    public string? Artist { get; init; }
}
