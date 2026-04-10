namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyPlaylistListResponseDto
{
    public int Total { get; init; }
    public List<SpotifyPlaylistSummaryDto> Items { get; init; } = [];
}
