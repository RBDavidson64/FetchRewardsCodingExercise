using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.EntityModel;
using Microsoft.AspNetCore.Mvc;

namespace FetchRewardsApi.Interfaces;

public interface IAddPoints
{
    /// <summary>
    /// Asynchronously adds points to a payer's balance and a new <see cref="PointTransaction"/> and <see cref="AvailablePoints"/> entity records.
    /// </summary>
    /// <param name="addPointsTransaction">Specifies the payer, points, and transaction timestamp used to add and update entity records.</param>
    /// <param name="cancellationToken">A means of cancelling the operations.</param>
    /// <returns>Either a <see cref="ProblemDetails"/> record specifying the reason the command failed, or <see cref="StatusCodes.Status200OK"/></returns>
    public Task<IResult> AddPoints(AddPointsTransaction addPointsTransaction, CancellationToken cancellationToken);
}