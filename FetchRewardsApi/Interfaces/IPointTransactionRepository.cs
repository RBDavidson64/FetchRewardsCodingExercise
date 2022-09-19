using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.Interfaces;

internal interface IPointTransactionRepository 
{
    /// <summary>
    /// Add a new <see cref="PointTransaction"/> entity record to the context.
    /// </summary>
    /// <param name="pointTransaction">The entity record to add to the context.</param>
    void                                Add(PointTransaction  pointTransaction);
}