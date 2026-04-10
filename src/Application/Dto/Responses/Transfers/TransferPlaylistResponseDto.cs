namespace Application.Dto.Responses.Transfers;

public record TransferPlaylistResponseDto
{
    public Guid Id { get; init; }
    public string SourcePlaylistId { get; init; } = string.Empty;
    public string? TargetPlaylistId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ArtworkUrl { get; init; }
    public string? TargetPlaylistUrl { get; init; }
    public bool IsMerged { get; init; }
    public ICollection<TransferTrackResponseDto> Tracks { get; init; } = [];
}
