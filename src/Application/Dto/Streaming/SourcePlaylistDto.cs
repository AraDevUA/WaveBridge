namespace Application.Dto.Streaming;

public record SourcePlaylistDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
