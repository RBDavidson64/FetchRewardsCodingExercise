using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.Interfaces;

internal interface ISpentPointsRepository
{
    /// <summary>
    /// Add a new <see cref="SpentPoints"/> entity record to the context.
    /// </summary>
    /// <param name="spentPoints">The entity record to add to the context.</param>
    void Add(SpentPoints spentPoints);
}