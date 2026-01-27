namespace Application.Dto.Streaming.Responses.Spotify;
public record SpotifySearchResponseDto
{
    public SpotifyTracksContainerDto Tracks { get; set; } = null!;
}
