namespace Domain.Entities.Interfaces;

public interface ISoftDeletableEntity
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedUtc { get; set; }
}
