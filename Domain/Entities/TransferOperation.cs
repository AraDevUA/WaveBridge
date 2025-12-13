using Domain.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities;

public class TransferOperation : IAuditableEntity, ISoftDeletableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public StreamingService SourceService { get; set; }
    public StreamingService TargetService { get; set; }
    public TransferStatus Status { get; set; }

    public DateTimeOffset? CreatedUtc { get; set; }
    public DateTimeOffset? ModifiedUtc { get; set; }
    public DateTimeOffset? DeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<TransferItem> TransferItems { get; set; }
    TransferOperation()
    {
        TransferItems = []; 
    }
}
