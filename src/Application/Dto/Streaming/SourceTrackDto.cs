namespace Application.Dto.Streaming;

public record SourceTrackDto
{
    public string SourceId { get; init; }
    public string TrackName { get; init; } 
    public string Artist { get; init; }
    public string Album { get; init; }
}
