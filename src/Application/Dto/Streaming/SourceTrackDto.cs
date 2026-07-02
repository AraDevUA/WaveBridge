namespace Application.Dto.Streaming;

public record SourceTrackDto
{
    public string SourceId { get; init; } = string.Empty;
    public string TrackName { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string? Album { get; init; }
    public string? ArtworkUrl { get; init; }
}
