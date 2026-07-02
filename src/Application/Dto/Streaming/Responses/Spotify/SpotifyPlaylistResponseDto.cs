namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyPlaylistResponseDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<SpotifyImageDto> Images { get; init; } = [];
    public SpotifyExternalUrlsDto? ExternalUrls { get; init; }
    public SpotifyPlaylistTracksSummaryDto? Tracks { get; init; }
}
