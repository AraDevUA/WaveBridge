using Domain.Entities.Interfaces;
using Shared.Enums;

namespace Domain.Entities;

public class TransferOperation : IAuditableEntity, ISoftDeletableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public StreamingService SourceService { get; set; }
    public StreamingService? TargetService { get; set; }
    public TransferStatus Status { get; set; }
    public bool IsPublic { get; set; }
    public bool ToSinglePlaylist { get; set; }
    public string? MergedTargetPlaylistId { get; set; }
    public string? MergedTargetPlaylistUrl { get; set; }
    public DateTimeOffset? StartedUtc { get; set; }
    public DateTimeOffset? CompletedUtc { get; set; }
    public DateTimeOffset? CreatedUtc { get; set; }
    public DateTimeOffset? ModifiedUtc { get; set; }
    public DateTimeOffset? DeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<TransferPlaylist> TransferPlaylists { get; set; } = [];
}
