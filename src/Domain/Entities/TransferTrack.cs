using Shared.Enums;

namespace Domain.Entities;

public class TransferTrack
{
    public Guid Id { get; set; }
    public Guid TransferPlaylistId { get; set; }
    public TransferTrackStatus Status { get; set; }
    public string SourceId { get; set; } = string.Empty;
    public string? TargetId { get; set; }
    public string TrackName { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string? Album { get; set; }
    public string? ArtworkUrl { get; set; }
    public string? ErrorMessage { get; set; }

    public virtual TransferPlaylist TransferPlaylist { get; set; } = null!;
}
