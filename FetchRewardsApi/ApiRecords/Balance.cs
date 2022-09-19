using FetchRewardsApi.Commands;

namespace FetchRewardsApi.ApiRecords;

/// <summary>
/// An individual payer's balance, returned as a collection element from the <see cref="GetBalancesCommand"/>
/// </summary>
/// <param name="Payer"></param>
/// <param name="Points"></param>
internal sealed record  Balance(string Payer, int Points);