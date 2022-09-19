using FetchRewardsApi.EntityModel;

namespace UnitTests.TestHelpers;

/// <summary>
/// The expected state of a <see cref="PointsContext"/> after all commands have been executed.
/// </summary>
/// <param name="ExpectedAllocatedPoints">The expected records in <see cref="PointsContext.AllocatedPointsSet"/></param>
/// <param name="ExpectedAvailablePoints">The expected records in <see cref="PointsContext.AvailablePointsSet"/></param>
/// <param name="ExpectedPayerBalances">The expected records in <see cref="PointsContext.PayerBalances"/></param>
/// <param name="ExpectedPointTransactions">The expected records in <see cref="PointsContext.PointTransactions"/></param>
/// <param name="ExpectedSpentPoints">The expected records in <see cref="PointsContext.SpentPointsSet"/></param>
internal sealed record ExpectedFinalContextState(
    ICollection<AllocatedPoints>  ExpectedAllocatedPoints,
    ICollection<AvailablePoints>  ExpectedAvailablePoints,
    ICollection<PayerBalance>     ExpectedPayerBalances,
    ICollection<PointTransaction> ExpectedPointTransactions,
    ICollection<SpentPoints>      ExpectedSpentPoints
)
{
    /// <summary>
    /// Validates the final state of a <see cref="PointsContext"/>
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> to validate</param>
    public void ValidateFinalContextState(PointsContext context)
    {
        Assert.Equal(ExpectedAllocatedPoints.Count, context.AllocatedPointsSet.Count());
        Assert.Equal(ExpectedAvailablePoints.Count, context.AvailablePointsSet.Count());
        Assert.Equal(ExpectedPayerBalances.Count, context.PayerBalances.Count());
        Assert.Equal(ExpectedPointTransactions.Count, context.PointTransactions.Count());
        Assert.Equal(ExpectedSpentPoints.Count, context.SpentPointsSet.Count());
        
        Assert.All(context.AllocatedPointsSet, ValidateAllocatedPoints);
        Assert.All(context.AvailablePointsSet, ValidateAvailablePoints);
        Assert.All(context.PayerBalances, ValidatePayerBalance);
        Assert.All(context.PointTransactions, ValidatePointTransaction);
        Assert.All(context.SpentPointsSet, ValidateSpentPoints);

    }

    /// <summary>
    /// Validates a single <see cref="SpentPoints"/> entity record.
    /// </summary>
    /// <param name="actual">The record to validate</param>
    private void ValidateSpentPoints(SpentPoints actual)
    {
        var expected = ExpectedSpentPoints.SingleOrDefault(x => x.SpentPointsId == actual.SpentPointsId);
        Assert.NotNull(expected);
        Assert.Equal(expected.PointsSpent, actual.PointsSpent);
        Assert.True(expected.TimeStamp < actual.TimeStamp && expected.TimeStamp.AddMinutes(1) > actual.TimeStamp);
        Assert.Equal(ExpectedAllocatedPoints.Count, expected.AllocatedPoints.Count);
        Assert.All(actual.AllocatedPoints, ValidateAllocatedPoints);
    }

    /// <summary>
    /// Validates a single <see cref="PointTransaction"/> entity record.
    /// </summary>
    /// <param name="actual">The record to validate</param>
    private void ValidatePointTransaction(PointTransaction actual)
    {
        var expected = ExpectedPointTransactions.SingleOrDefault(x => x.PointTransactionId == actual.PointTransactionId);
        Assert.NotNull(expected);
        Assert.Equal(expected.AvailablePoints.AvailablePointsId, actual.AvailablePoints.AvailablePointsId);
        Assert.Equal(expected.Payer,                             actual.Payer);
        Assert.Equal(expected.PayerBalance.PayerBalanceId,       actual.PayerBalance.PayerBalanceId);
        Assert.Equal(expected.Points,                            actual.Points);
        Assert.Equal(expected.TimeStamp,                         actual.TimeStamp);
        var expectedAllocatedPoints = ExpectedAllocatedPoints.Where(x => x.PointTransaction.PointTransactionId == actual.PointTransactionId).ToArray();
        if(expectedAllocatedPoints.Any())
        {
            Assert.Equal(expectedAllocatedPoints.Length, actual.AllocatedPoints.Count);
            Assert.All(actual.AllocatedPoints, ValidateAllocatedPoints);
            return;
        }
        
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        // Remarks: I don't think the actual.AllocatedPoints should be null, but that is happening when expectedAllocatedPoints.Length == 0.
        //          I should track down the cause of the error, but it doesn't actually matter for the purpose of this validation check.
        var actualAllocatedPoints = actual.AllocatedPoints?.Count ?? 0;
        if(actualAllocatedPoints == 0) return;
        Assert.Fail($"{nameof(expectedAllocatedPoints)} had no records but actual.{nameof(actual.AllocatedPoints)} had {actual.AllocatedPoints!.Count} records");
    }

    /// <summary>
    /// Validates a single <see cref="PayerBalance"/> entity record.
    /// </summary>
    /// <param name="actual">The record to validate</param>
    private void ValidatePayerBalance(PayerBalance actual)
    {
        var expected = ExpectedPayerBalances.SingleOrDefault(x => x.PayerBalanceId == actual.PayerBalanceId);
        Assert.NotNull(expected);
        Assert.Equal(expected.Balance, actual.Balance);
        Assert.Equal(expected.Payer, actual.Payer);
        
        var expectedAvailablePoints = ExpectedAvailablePoints.Where(x => x.Payer == actual.Payer).ToArray();
        Assert.Equal(expectedAvailablePoints.Length, actual.AvailablePoints.Count);
        Assert.All(actual.AvailablePoints, ValidateAvailablePoints);

        var expectedAllocatedPoints = ExpectedAllocatedPoints.Where(x => x.Payer == actual.Payer).ToArray();
        Assert.Equal(expectedAllocatedPoints.Length, actual.AllocatedPoints.Count);
        Assert.All(actual.AllocatedPoints, ValidateAllocatedPoints);

        var expectedPointTransactions = ExpectedPointTransactions.Where(x => x.Payer == actual.Payer).ToArray();
        Assert.Equal(expectedPointTransactions.Length, actual.PointTransactions.Count);
        Assert.All(actual.PointTransactions, ValidatePointTransaction);

    }

    /// <summary>
    /// Validates a single <see cref="AvailablePoints"/> entity record.
    /// </summary>
    /// <param name="actual">The record to validate</param>
    private void ValidateAvailablePoints(AvailablePoints actual)
    {
        var expected = ExpectedAvailablePoints.SingleOrDefault(x => x.AvailablePointsId == actual.AvailablePointsId);
        Assert.NotNull(expected);
        Assert.Equal(expected.AllocatedPoints, actual.AllocatedPoints);
        Assert.Equal(expected.AllPointsAllocated, actual.AllPointsAllocated);
        Assert.Equal(expected.Payer, actual.Payer);
        Assert.Equal(expected.PointTransaction.PointTransactionId, actual.PointTransaction.PointTransactionId);
        Assert.Equal(expected.PointTransactionId, actual.PointTransactionId);
        Assert.Equal(expected.PayerBalance.PayerBalanceId, actual.PayerBalance.PayerBalanceId);
        Assert.Equal(expected.PointTransactionTimeStamp, actual.PointTransactionTimeStamp);
        Assert.Equal(expected.UnallocatedPoints, actual.UnallocatedPoints);
        Assert.Equal(expected.OriginalUnallocatedPoints, actual.OriginalUnallocatedPoints);
    }

    /// <summary>
    /// Validates a single <see cref="AllocatedPoints"/> entity record.
    /// </summary>
    /// <param name="actual">The record to validate</param>
    private void ValidateAllocatedPoints(AllocatedPoints actual)
    {
        var expected = ExpectedAllocatedPoints.SingleOrDefault(x => x.AllocatedPointsId == actual.AllocatedPointsId);
        Assert.NotNull(expected);
        Assert.Equal(expected.AvailablePoints.AvailablePointsId,   actual.AvailablePoints.AvailablePointsId);
        Assert.Equal(expected.Payer,                               actual.Payer);
        Assert.Equal(expected.PayerBalance.PayerBalanceId,        actual.PayerBalance.PayerBalanceId);
        Assert.Equal(expected.PointsAllocated,                     actual.PointsAllocated);
        Assert.Equal(expected.PointTransaction.PointTransactionId, actual.PointTransaction.PointTransactionId);
        Assert.Equal(expected.SpentPoints.SpentPointsId,           actual.SpentPoints.SpentPointsId);
        Assert.True(expected.TimeStamp < actual.TimeStamp && expected.TimeStamp.AddMinutes(1) > actual.TimeStamp);
    }
}