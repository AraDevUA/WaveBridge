namespace Domain.Entities;

public class TransferPlaylist
{
    public Guid Id { get; set; }
    public Guid TransferOperationId { get; set; }
    public string SourcePlaylistId { get; set; } = string.Empty;
    public string? TargetPlaylistId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ArtworkUrl { get; set; }
    public string? TargetPlaylistUrl { get; set; }

    public virtual TransferOperation TransferOperation { get; set; } = null!;
    public virtual ICollection<TransferTrack> TransferTracks { get; set; } = [];
}
