namespace Application.Dto.Streaming.Responses.Spotify;
public record SpotifyLikedTracksResponseDto
{
    public int Total { get; set; }
    public List<SpotifyPlaylistItemDto> Items { get; set; } = [];
}