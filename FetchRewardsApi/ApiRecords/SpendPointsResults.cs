using FetchRewardsApi.Commands;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.ApiRecords;

/// <summary>
/// A collection of <see cref="SpendPointsResult"/> records, returned by the <see cref="SpendPointsCommand"/>
/// </summary>
internal readonly struct SpendPointsResults : ISpendPointsResults
{
    private readonly Dictionary<string, SpendPointsResult> _resultsByPayer = new();
    public SpendPointsResults() {
    }

    /// <inheritdoc />
    public void Subtract(string payer, int points)
    {
        if (!_resultsByPayer.TryGetValue(payer, out var spendPointsResult))
            spendPointsResult = new SpendPointsResult(payer, 0);

        spendPointsResult                      = spendPointsResult with { Points = spendPointsResult.Points - points };
        _resultsByPayer[payer] = spendPointsResult;
    }

    /// <inheritdoc />
    public SpendPointsResult[] Results => _resultsByPayer.Values.ToArray();
}