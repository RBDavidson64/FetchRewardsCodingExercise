using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.Interfaces;

internal interface ICreateAvailablePoints
{
    /// <summary>
    /// Creates a new <see cref="AvailablePoints"/> entity record.
    /// </summary>
    /// <param name="pointTransaction">The <see cref="PointTransaction"/> entity record used to create the new <see cref="AvailablePoints"/> entity record.</param>
    /// <returns>The new entity record</returns>
    AvailablePoints CreateAvailablePoints(PointTransaction pointTransaction);
}