using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories.Contracts;

internal interface IRepository<TEntity, TKey> where TEntity : class
{
    public IQueryable<TEntity> All { get; }
    public Task<bool> SaveChangesAsync();
    public Task<TEntity?> FindAsync(params object?[]? keyValue);
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task CreateAsync(TEntity entity);
    public Task AddAsync(TEntity entity);
    public Task UpdateAsync(TEntity entity);
    public Task DeleteAsync(TEntity entity);
    public Task<IDbContextTransaction> BeginTransactionAsync();
}
