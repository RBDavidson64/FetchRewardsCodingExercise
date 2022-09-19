using FetchRewardsApi.Commands;

namespace FetchRewardsApi.ApiRecords;

/// <summary>
/// The net changes made to a payer's balance. Returned as a collection element by the <see cref="SpendPointsCommand"/>
/// </summary>
/// <param name="Payer">The payer whose balance was changed.</param>
/// <param name="Points">The net change made to the payer's balance</param>
public sealed record SpendPointsResult(string Payer, int Points);