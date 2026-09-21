using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

/// <summary>
/// Entity Framework implementation of <see cref="IRepository{T}"/>.
/// Write operations stage changes only; callers decide when to persist by
/// calling <see cref="SaveChangesAsync"/>.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Set;

    public Repository(AppDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
        => await Set.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        => await Set.AsNoTracking().ToListAsync();

    public virtual async Task AddAsync(T entity)
        => await Set.AddAsync(entity);

    public virtual void Update(T entity)
        => Set.Update(entity);

    public virtual void Delete(T entity)
        => Set.Remove(entity);

    public virtual Task<int> SaveChangesAsync()
        => Context.SaveChangesAsync();
}
