using FetchRewardsApi.Commands;

namespace FetchRewardsApi.ApiRecords;

/// <summary>
/// The points passed to the <see cref="SpendPointsCommand"/>
/// </summary>
/// <param name="Points">The points to spend.</param>
public sealed record SpendPointsTransaction(int Points);