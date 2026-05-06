using Shared.Enums;

namespace Application.Dto.Responses.Transfers;

public record TransferHistoryItemResponseDto
{
    public Guid Id { get; init; }
    public StreamingService SourceService { get; init; }
    public StreamingService? TargetService { get; init; }
    public TransferStatus Status { get; init; }
    public bool IsPublic { get; init; }
    public bool ToSinglePlaylist { get; init; }
    public string? MergedTargetPlaylistUrl { get; init; }
    public DateTimeOffset? StartedUtc { get; init; }
    public DateTimeOffset? CompletedUtc { get; init; }
    public int PlaylistCount { get; init; }
}
