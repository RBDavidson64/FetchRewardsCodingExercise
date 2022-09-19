using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;

namespace UnitTests.TestHelpers;

/// <summary>
/// A <see cref="SpendPointsTransaction"/> and the expected payer balances after the <see cref="SpendPointsTransaction"/> has been processed.
/// </summary>
/// <param name="SpendPointsTransaction">The <see cref="SpendPointsTransaction"/> which will be processed by a <see cref="SpendPointsCommand"/></param>
/// <param name="ExpectedResultsByPayer">The expected payer balances after the <paramref name="SpendPointsTransaction"/> has been processed.</param>
internal sealed record SpendPointTransactionAndExpectedResults(
    SpendPointsTransaction   SpendPointsTransaction,
    IDictionary<string, int> ExpectedResultsByPayer);