using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Inspectors;

public class SoftDeletableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        MarkEntitiesAsDeleted(eventData);
        return result;
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        MarkEntitiesAsDeleted(eventData);
        return new ValueTask<InterceptionResult<int>>(result);
    }
    private void MarkEntitiesAsDeleted(DbContextEventData eventData)
    {
        if (eventData.Context is not null)
        {
            var deletedEntries = eventData.Context.ChangeTracker.Entries<ISoftDeletableEntity>()
                .Where(entry => entry.State == EntityState.Deleted);

            foreach (var entry in deletedEntries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedUtc = DateTime.UtcNow;
            }
        }
    }
}
