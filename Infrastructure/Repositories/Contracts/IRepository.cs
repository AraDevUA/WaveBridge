using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories.Contracts;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    public IQueryable<TEntity> All { get; }
    public Task<bool> SaveChangesAsync();
    public Task<TEntity?> FindAsync(CancellationToken cancellationToken = default, params object?[]? keyValue);
    public Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
