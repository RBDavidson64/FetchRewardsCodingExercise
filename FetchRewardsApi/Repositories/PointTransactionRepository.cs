using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Repositories;

/// <summary>
/// A repository used to add new <see cref="PointTransaction"/> entity records.
/// </summary>
internal sealed class PointTransactionRepository
    : IPointTransactionRepository
{
    private readonly GenericRepository<PointTransaction> _repository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> used to read, add, and update entity records.</param>
    public PointTransactionRepository(PointsContext                        context) => _repository = new GenericRepository<PointTransaction>(context);

    /// <inheritdoc />
    public       void                                Add(PointTransaction  pointTransaction)  => _repository.Add(pointTransaction);
}