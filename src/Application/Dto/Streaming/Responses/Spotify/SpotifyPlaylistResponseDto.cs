namespace Application.Dto.Streaming.Responses.Spotify;

public record SpotifyPlaylistResponseDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
