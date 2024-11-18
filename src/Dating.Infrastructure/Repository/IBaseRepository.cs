using System.Linq.Expressions;

namespace NEFORmal.ua.Dating.Infrastructure.Repository;

public interface IBaseRepository<T>
{
    ValueTask<T?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken,
        bool asNoTracking);

    void Insert(T item);

    void Update(T item);

    Task DeleteAsync(object id, CancellationToken cancellationToken);

    void Delete(T item);

    Task<List<T>> Get(
        Expression<Func<T, bool>>? filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
        CancellationToken cancellationToken,
        bool asNoTracking,
        string includeProperties,
        int skip,
        int take
    );
}
