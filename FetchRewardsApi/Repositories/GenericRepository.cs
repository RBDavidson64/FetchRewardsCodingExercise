using System.Linq.Expressions;
using FetchRewardsApi.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Repositories;

/// <summary>
/// A generic repository which is used a a backing variable by other repositories.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
internal sealed class GenericRepository<TEntity> 
    where TEntity : class
{
    private readonly PointsContext     _context;
    private readonly DbSet<TEntity>    _dbSet;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> used to read, add, and update entity records.</param>
    internal GenericRepository(PointsContext context)
    {
        _context = context;
        _dbSet   = context.Set<TEntity>();
    }

    /// <summary>
    /// Creates an <see cref="IQueryable{TEntity}"/>
    /// </summary>
    /// <param name="filter">An optional method to filter returned records.</param>
    /// <param name="orderBy">An optional method to sort the returned records. </param>
    /// <returns>An <see cref="IQueryable"/> which can be executed to retrieve the desired records in sort order.</returns>
    public IQueryable<TEntity> CreateQuery(
        Expression<Func<TEntity, bool>>?                       filter  = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
    {
        var query                      = filter is null ? _dbSet : _dbSet.Where(filter);
        if (orderBy is not null) query = orderBy(query);
        return query;
    }

    /// <summary>
    /// An asynchronous means to retrieve a collection of entity records.
    /// </summary>
    /// <param name="cancellationToken">A means of cancelling the operation</param>
    /// <param name="filter">An optional method to filter returned records.</param>
    /// <param name="orderBy">An optional method to sort the returned records. </param>
    /// <returns>An <see cref="ICollection{TEntity}"/> of the desired records in sort order.</returns>
    public  async Task<ICollection<TEntity>> Get(
        CancellationToken                                      cancellationToken,
        Expression<Func<TEntity, bool>>?                       filter  = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
        => await CreateQuery(filter, orderBy).ToListAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// An asynchronous means to sum up a value in a set of entity records. 
    /// </summary>
    /// <param name="selector">A method which selects the property which will be summed.</param>
    /// <param name="cancellationToken">A means of cancelling the operation.</param>
    /// <returns>The sum of the values in the specified property.</returns>
    internal  async Task<int> Sum(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken) => await _dbSet.SumAsync(selector, cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Get a single entity record or null.
    /// </summary>
    /// <param name="filter">A method to filter the entity records.</param>
    /// <param name="cancellationToken">A means of cancelling the operation</param>
    /// <returns>Either the single record which matches the <paramref name="filter"/> conditions, or null</returns>
    public  async Task<TEntity?> SingleOrDefault(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken) => await _dbSet.SingleOrDefaultAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Add an entity record to the context.
    /// </summary>
    /// <param name="entity">The entity record to add.</param>
    /// <remarks>
    /// Only entities in a <see cref="EntityState.Detached"/> or <see cref="EntityState.Unchanged"/> state will be changed to an <see cref="EntityState.Added"/> state. 
    /// </remarks>
    public void Add(TEntity entity)
    {
        var entityEntry = _context.Entry(entity);

        switch (entityEntry.State)
        {
            case EntityState.Detached:
                _dbSet.Add(entity);
                return;
            case EntityState.Unchanged:
                entityEntry.State = EntityState.Added;
                return;
            case EntityState.Deleted:  return;
            case EntityState.Modified: return;
            case EntityState.Added:    return;
            default:                   return;
        }
    }

    /// <summary>
    /// Marks an entity record already in the context as <see cref="EntityState.Modified"/>.
    /// </summary>
    /// <param name="entity">The entity record to mark as modified.</param>
    /// <remarks>
    /// Only entities in a <see cref="EntityState.Detached"/> or <see cref="EntityState.Unchanged"/> state will be changed to an <see cref="EntityState.Modified"/> state. 
    /// </remarks>
    internal  void Update(TEntity entity)
    {
        var entityEntry = _context.Entry(entity);
        
        switch (entityEntry.State)
        {
            case EntityState.Detached:
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                return;
            case EntityState.Unchanged: 
                entityEntry.State = EntityState.Modified;
                return;
            case EntityState.Deleted:  return;
            case EntityState.Modified: return;
            case EntityState.Added:    return;
            default:                   return;
        }
    }
}