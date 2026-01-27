namespace Application.Dto.Streaming.Responses.Spotify;
public record SpotifyPlaylistItemDto
{
    public SpotifyTrackDto? Track { get; init; }
}
