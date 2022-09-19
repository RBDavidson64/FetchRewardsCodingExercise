using FetchRewardsApi.EntityModel;
using Microsoft.AspNetCore.Mvc;

namespace FetchRewardsApi.Interfaces;

internal interface IAdjustPayerPoints
{
    /// <summary>
    /// Updates or adds a <see cref="PayerBalance.Balance"/> entity record. 
    /// </summary>
    /// <param name="payer">
    /// The payer whose <see cref="PayerBalance"/> entity record will be modified.
    /// <remarks>
    /// If the payer does not have a <see cref="PayerBalance"/> entity record, a new record will be added with <see cref="PayerBalance.Balance"/>
    /// set to <paramref name="pointToAdd"/>
    /// </remarks>
    /// </param>
    /// <param name="pointToAdd">
    /// The points to add to the payer's balance
    /// <value>
    /// Negative values are allowed as long as they do not result in <see cref="PayerBalance.Balance"/> becoming negative.
    /// </value>
    /// </param>
    /// <param name="cancellationToken">A means of cancelling to operation.</param>
    /// <returns>
    /// A <see cref="Tuple"/> containing either the updated <see cref="PayerBalance"/> entity record an a null <see cref="IResult"/>;
    /// or, if the operation would result in a negative <see cref="PayerBalance.Balance"/>,  a null <see cref="PayerBalance"/>
    /// entity record and a <see cref="ProblemDetails"/> <see cref="IResult"/> detailing to error.
    /// </returns>
    Task<(PayerBalance? PayerBalance, IResult? Problem)> AdjustPayerPoints(string payer, int pointToAdd, CancellationToken cancellationToken);
}