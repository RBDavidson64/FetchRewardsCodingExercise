using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;

namespace UnitTests.TestHelpers;

/// <summary>
/// Records to add to <see cref="PointsContext"/> before <see cref="SpendPointsCommand"/> is called one or more times.
/// </summary>
/// <param name="PointTransactions">The records to add to <see cref="PointsContext.PointTransactions"/></param>
/// <param name="AvailablePoints"> The records to add to <see cref="PointsContext.AvailablePointsSet"/>. </param>
/// <param name="PayerBalances">The records to add to <see cref="PointsContext.PayerBalances"/>. </param>
internal sealed record ContextStateBeforeRunningSpendPointsCommand(
    ICollection<PointTransaction> PointTransactions,
    ICollection<AvailablePoints> AvailablePoints,
    ICollection<PayerBalance>    PayerBalances
)
{
    
    /// <summary>
    /// Configures a <see cref="PointsContext"/>
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> which will have records added to it.</param>
    public void ConfigureContextState(PointsContext context)
    {
        context.PointTransactions.AddRange(PointTransactions);
        context.AvailablePointsSet.AddRange(AvailablePoints);
        context.PayerBalances.AddRange(PayerBalances);
        context.SaveChanges();
    }
}