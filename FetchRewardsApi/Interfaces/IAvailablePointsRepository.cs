using FetchRewardsApi.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Interfaces;

internal interface IAvailablePointsRepository
{
    /// <summary>
    /// Add a new <see cref="AvailablePoints"/> entity record to the context.
    /// </summary>
    /// <param name="availablePoints">The entity record to add to the context.</param>
    void                         Add(AvailablePoints                     availablePoints);
    
    /// <summary>
    /// Get an <see cref="IEnumerable{AvailablePoints}"/> which can be used to allocate <see cref="SpentPoints"/>
    /// </summary>
    /// <param name="pointsToAllocate">The points which are to be allocated.</param>
    /// <returns>
    /// An <see cref="IEnumerable{AvailablePoints}"/> in ascending <see cref="AvailablePoints.PointTransactionTimeStamp"/> order
    /// whose <see cref="AvailablePoints.UnallocatedPoints"/> are non-zero and whose sum is equal to or greater than the <paramref name="pointsToAllocate"/>
    /// </returns>
    IEnumerable<AvailablePoints> UnallocatedPointsOrderedByTimeStamp(int pointsToAllocate);
    
    /// <summary>
    /// Marks an existing <see cref="AvailablePoints"/> entity record as <see cref="EntityState.Modified"/>
    /// </summary>
    /// <param name="availablePoints">The entity record to mark as <see cref="EntityState.Modified"/></param>
    void                              Update(AvailablePoints availablePoints);
}