using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Repositories;

/// <summary>
/// A repository used to read, add, and update <see cref="AvailablePoints"/> entity records.
/// </summary>
internal sealed class AvailablePointsRepository
    : 
      IAvailablePointsRepository
{
    private readonly GenericRepository<AvailablePoints> _repository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> used to read, add, and update entity records.</param>
    public AvailablePointsRepository(PointsContext context) => _repository             = new GenericRepository<AvailablePoints>(context);

    /// <inheritdoc />
    public void Add(AvailablePoints                availablePoints) => _repository.Add(availablePoints);

    /// <inheritdoc />
    public IEnumerable<AvailablePoints> UnallocatedPointsOrderedByTimeStamp(int pointsToAllocate)
    {
        if (pointsToAllocate <= 0) yield break;

        var availablePointsEnumerable = _repository.CreateQuery(
                                                                filter: availablePoints => !availablePoints.AllPointsAllocated,
                                                                orderBy: query => query.OrderBy(availablePoints => availablePoints.PointTransactionTimeStamp)
                                                               )
                                                   .Include(availablePoints => availablePoints.PayerBalance);

        var totalUnallocatedPoints = 0;
        foreach (var availablePoints in availablePointsEnumerable)
        {
            totalUnallocatedPoints += availablePoints.UnallocatedPoints;
            yield return availablePoints;
            if(totalUnallocatedPoints > pointsToAllocate) yield break;
        }
    }

    /// <inheritdoc />
    public void Update(AvailablePoints availablePoints) => _repository.Update(availablePoints);
}