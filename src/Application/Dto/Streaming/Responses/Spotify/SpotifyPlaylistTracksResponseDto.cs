namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyPlaylistTracksResponseDto
{
    public List<SpotifyPlaylistItemDto> Items { get; init; } = [];
}
