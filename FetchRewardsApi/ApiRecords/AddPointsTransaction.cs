using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.ApiRecords;

/// <summary>
/// Parameters for the <see cref="AddPointsCommand"/>
/// </summary>
/// <param name="Payer">The payer where the points are to be added</param>
/// <param name="Points">
/// The points to add.
/// <value>
/// A negative value is allowed, so long as this does not result in a negative <see cref="PayerBalance.Balance"/>
/// </value>
/// </param>
/// <param name="TimeStamp">The <see cref="DateTime"/> when the points are recorded as being posted.</param>
public sealed record AddPointsTransaction(string Payer, int Points, DateTime TimeStamp);