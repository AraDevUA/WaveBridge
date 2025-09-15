using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Inspectors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateModifiedUtc(eventData);
        return result;
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateModifiedUtc(eventData);
        return new ValueTask<InterceptionResult<int>>(result);
    }
    private void UpdateModifiedUtc(DbContextEventData data)
    {
        if (data.Context is not null)
        {
            var modifiedEntries = data.Context.ChangeTracker.Entries<IAuditableEntity>()
                .Where(entry => entry.State == EntityState.Modified);

            foreach (var entry in modifiedEntries)
                entry.Entity.ModifiedUtc = DateTime.UtcNow;
        }
    }
}
