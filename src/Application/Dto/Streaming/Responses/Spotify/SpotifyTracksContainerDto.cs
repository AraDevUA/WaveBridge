namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyTracksContainerDto
{
    public List<SpotifySearchItemDto> Items { get; set; } = [];
}
