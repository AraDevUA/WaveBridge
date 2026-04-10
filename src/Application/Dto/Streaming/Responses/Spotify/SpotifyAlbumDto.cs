namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyAlbumDto
{
    public string Name { get; init; } = string.Empty;
    public List<SpotifyImageDto> Images { get; init; } = [];
}
