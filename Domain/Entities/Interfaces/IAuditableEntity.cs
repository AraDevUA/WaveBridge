namespace Domain.Entities.Interfaces;
public interface IAuditableEntity
{
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset? ModifiedUtc { get; set; }
}
