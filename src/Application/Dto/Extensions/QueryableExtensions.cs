using System.Linq.Expressions;

namespace Application.Dto.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (condition)
            return source.Where(predicate);

        return source;
    }
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
        => query.Skip(page * pageSize).Take(pageSize);
}
