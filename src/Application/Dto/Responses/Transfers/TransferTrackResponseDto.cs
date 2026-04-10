using Shared.Enums;

namespace Application.Dto.Responses.Transfers;

public record TransferTrackResponseDto
{
    public Guid Id { get; init; }
    public string SourceId { get; init; } = string.Empty;
    public string? TargetId { get; init; }
    public string TrackName { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string? Album { get; init; }
    public string? ArtworkUrl { get; init; }
    public TransferTrackStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
}
