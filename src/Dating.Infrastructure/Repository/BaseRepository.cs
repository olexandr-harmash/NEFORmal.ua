
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace NEFORmal.ua.Dating.Infrastructure.Repository;

public abstract class BaseRepository<C, T> : IBaseRepository<T>
    where T : class
    where C : DbContext
{
    protected readonly C _context; // Контекст базы данных
    private readonly DbSet<T> _dbSet; // DbSet для сущностей типа T

    // Конструктор, который инициализирует контекст базы данных
    public BaseRepository(C context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual Task<List<T>> Get(
        Expression<Func<T, bool>>? filter = default,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = default,
        CancellationToken cancellationToken = default,
        bool asNoTracking = true,
        string includeProperties = "",
        int skip = default,
        int take = int.MaxValue)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Подключение связанных данных (если указаны)
        foreach (var includeProperty in includeProperties.Split
            (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        // Включение NoTracking, если указано
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        // Применение сортировки (если указана)
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        // Пагинация (skip и take)
        query = query.Skip(skip).Take(take);

        // Выполнение запроса с использованием cancellationToken
        return query.ToListAsync(cancellationToken);
    }

    public virtual ValueTask<T?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        IQueryable<T> query = _dbSet;
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        return _dbSet.FindAsync(id, cancellationToken);
    }

    public virtual void Insert(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(T entityToUpdate)
    {
        _dbSet.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public virtual async Task DeleteAsync(object id, CancellationToken cancellationToken)
    {
        T? entityToDelete = await GetByIdAsync(id, cancellationToken);
        if (entityToDelete != null)
        {
            Delete(entityToDelete);
        }
    }

    public virtual void Delete(T entityToDelete)
    {
        if (_context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
        }
        _dbSet.Remove(entityToDelete);
    }
}
