using Shared.Enums;

namespace Application.Dto.Responses.Transfers;

public record TransferDetailsResponseDto
{
    public Guid Id { get; init; }
    public StreamingService Source { get; init; }
    public StreamingService? Destination { get; init; }
    public TransferStatus Status { get; init; }
    public bool IsPublic { get; init; }
    public bool ToSinglePlaylist { get; init; }
    public DateTimeOffset? StartedUtc { get; init; }
    public DateTimeOffset? CompletedUtc { get; init; }
    public string? MergedTargetPlaylistId { get; init; }
    public string? MergedTargetPlaylistUrl { get; init; }
    public ICollection<TransferPlaylistResponseDto> Playlists { get; init; } = [];
    public ICollection<TransferTrackResponseDto> FailedTracks { get; init; } = [];
}
