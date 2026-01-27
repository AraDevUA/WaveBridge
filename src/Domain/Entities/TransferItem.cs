using Domain.Enums;

namespace Domain.Entities;

public class TransferItem
{
    public Guid Id { get; set; }
    public Guid TransferOperationId { get; set; }
    public TransferItemStatus Status { get; set; }

    public string SourceId { get; set; }
    public string TargetId { get; set; }

    public string TrackName { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }

    public virtual TransferOperation TransferOperation { get; set; }
}
