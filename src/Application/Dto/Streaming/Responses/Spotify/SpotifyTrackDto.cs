namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyTrackDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public SpotifyAlbumDto? Album { get; init; }
    public List<SpotifyArtistDto> Artists { get; init; } = [];
}