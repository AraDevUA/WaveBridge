using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Inspectors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{  
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return result;
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return new ValueTask<InterceptionResult<int>>(result);
    }
    private void UpdateAuditableEntities(DbContext? context)
    {
        if(context is null)
            return;

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedUtc = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedUtc = utcNow;
                    break;
                default:
                    break;
            }
        }
    }
}
