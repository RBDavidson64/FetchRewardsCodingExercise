using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.EntityModel;
using Microsoft.AspNetCore.Mvc;

namespace FetchRewardsApi.Interfaces;

internal interface IAllocatePoints
{
    /// <summary>
    /// Processes <see cref="AvailablePoints"/> entity records with non-zero <see cref="AvailablePoints.UnallocatedPoints"/>,
    /// allocating points until all <paramref name="spentPoints.PointsSpent"/> have been allocated.
    /// </summary>
    /// <param name="spentPoints">The entity record specifying the points which are to be allocated.</param>
    /// <param name="cancellationToken">A means of cancelling the operation.</param>
    /// <returns>
    /// If successful,a <see cref="Tuple"/> containing a <see cref="bool"/> which is set to true, and an <see cref="Results.Ok"/> containing
    /// the <see cref="SpendPointsResults"/> detailing the net changes to payer's balances.
    ///
    /// If unsuccessful, a <see cref="Tuple"/> containing a <see cref="bool"/> which is set to false, and an <see cref="ProblemDetails"/>
    /// which specifies the cause of the failure.
    /// </returns>
    Task<(bool Success, IResult Result)> AllocatePoints( SpentPoints spentPoints, CancellationToken cancellationToken);
}