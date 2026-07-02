using System.Text.Json.Serialization;

namespace Application.Dto.Streaming.Responses.SoundCloud;

public record SoundCloudPlaylistListResponseDto
{
    public List<SoundCloudPlaylistResponseDto> Collection { get; init; } = [];

    [JsonPropertyName("total_results")]
    public int TotalResults { get; init; }
}
