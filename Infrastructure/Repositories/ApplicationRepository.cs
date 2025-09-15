namespace Infrastructure.Repositories;

public class ApplicationRepository<TEntity, TKey>(ApplicationDbContext context) : Repository<ApplicationDbContext, TEntity>(context) where TEntity : class
{
}
