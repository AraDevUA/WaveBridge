namespace Application.Dto.Streaming;

public record TrackSearchDto
{
    public string Name { get; init; }
    public string? Album { get; init; }
    public string Artist { get; init; }
}
