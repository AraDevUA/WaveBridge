using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
    public async Task<TEntity?> FindAsync(params object?[]? keyValue)
    {
        return await _dbSet.FindAsync(keyValue);
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    public async Task CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }
    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
