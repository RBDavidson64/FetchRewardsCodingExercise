using FetchRewardsApi.ApiRecords;
using Microsoft.AspNetCore.Mvc;

namespace FetchRewardsApi.Interfaces;

public interface ISpendPoints
{
    /// <summary>
    /// Allocates spent points.
    /// </summary>
    /// <param name="spendPointsTransaction">The <see cref="SpendPointsTransaction"/> api record specifying points to be allocated.</param>
    /// <param name="cancellationToken">A means of cancelling the operations.</param>
    /// <returns>
    /// An <see cref="IResult"/> which either specifies the net changes made to payer balances,
    /// or the <see cref="ProblemDetails"/> which prevented the command from allocating any points.
    /// </returns>
    public Task<IResult> SpendPoints(SpendPointsTransaction spendPointsTransaction, CancellationToken cancellationToken);
}