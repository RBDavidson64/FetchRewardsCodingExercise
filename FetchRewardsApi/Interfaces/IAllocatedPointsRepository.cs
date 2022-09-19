using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.Interfaces;

internal interface IAllocatedPointsRepository
{
    /// <summary>
    /// Add a new <see cref="AllocatedPoints"/> entity record to the context.
    /// </summary>
    /// <param name="allocatedPoints">The entity record to add to the context.</param>
    void Add(AllocatedPoints allocatedPoints);
}