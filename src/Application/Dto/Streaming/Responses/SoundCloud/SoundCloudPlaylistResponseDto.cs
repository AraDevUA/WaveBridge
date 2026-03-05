namespace Application.Dto.Streaming.Responses.SoundCloud;

public record SoundCloudPlaylistResponseDto
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public List<SoundCloudTrackDto> Tracks { get; init; } = new();
}