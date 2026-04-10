namespace Application.Dto.Requests.Transfers;

public record CreateTransferPlaylistRequestDto
{
    public string SourcePlaylistId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ArtworkUrl { get; init; }
    public ICollection<CreateTransferTrackRequestDto> Tracks { get; init; } = [];
}
