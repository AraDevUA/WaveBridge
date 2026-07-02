namespace Application.Dto.Streaming;

public record SourcePlaylistDto
{
    public string? SourceId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ArtworkUrl { get; init; }
    public string? Url { get; init; }
    public int? TrackCount { get; init; }
}
