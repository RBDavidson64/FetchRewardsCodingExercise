using FetchRewardsApi.EntityModel;
using Microsoft.AspNetCore.Mvc;

namespace FetchRewardsApi.Interfaces;

internal interface ICreateSpentPoints
{
    /// <summary>
    /// Creates a new <see cref="SpentPoints"/> entity record and adds it to the context.
    /// </summary>
    /// <param name="pointsToSpend">The points assigned to <see cref="SpentPoints.PointsSpent"/></param>
    /// <param name="cancellationToken">A means of cancelling the operations.</param>
    /// <returns>
    /// If <paramref name="pointsToSpend"/> is negative or greater than the available unallocated points,
    /// a <see cref="Tuple"/> is returned with a null <see cref="SpentPoints"/> entity record,
    /// and an <see cref="IResult"/> <see cref="ProblemDetails"/> specifying the error.
    ///
    /// Otherwise, a <see cref="Tuple"/> with the new <see cref="SpentPoints"/> entity record and a null <see cref="IResult"/>
    /// </returns>
    Task<(SpentPoints? SpentPoints, IResult? Problem)> CreateSpentPoints(int pointsToSpend, CancellationToken cancellationToken);
}