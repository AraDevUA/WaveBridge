using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;

namespace Infrastructure.Repositories;

public class Repository<TContext, TEntity> : IRepository<TEntity, Guid> 
    where TContext : DbContext
    where TEntity : class
{
    private readonly TContext _context;
    private readonly DbSet<TEntity> _dbSet;
    
    public Repository(TContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    public IQueryable<TEntity> All => _dbSet.AsQueryable();
    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    public async Task<TEntity?> FindAsync(CancellationToken cancellationToken = default, params object?[]? keyValue)
    {
        return await _dbSet.FindAsync(keyValue, cancellationToken);
    }
    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync();
    }
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }
    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }
}
