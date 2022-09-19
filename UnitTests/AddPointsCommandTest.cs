using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Repositories;
using Microsoft.AspNetCore.Http;
using UnitTests.TestHelpers;

namespace UnitTests;

/// <summary>
/// Validates the <see cref="AddPointsCommand"/>
/// </summary>
public sealed class AddPointsCommandTest
{
    /// <summary>
    /// Verifies that a single valid transaction is added correctly.
    /// </summary>
    /// <param name="addPointsTransaction">The transaction to add</param>
    /// <remarks>
    /// The in-memory <see cref="PointsContext"/> is fully recreated each time the method is run,
    /// meaning it starts with no records in any of its tables/collections.
    /// </remarks>
    [Theory]
    [MemberData(nameof(TestData.SamplePointTransactions_Individual_Successful), MemberType = typeof(TestData))]
    public async void Add_Individual_SuccessFull_Return_OkResult(AddPointsTransaction addPointsTransaction)
    {
        await using var context          = HelperMethods.CreatePointsContext();
        var             addPointsCommand = CreateAddPointsCommand(context);
        var             result           = await addPointsCommand.AddPoints(addPointsTransaction, CancellationToken.None).ConfigureAwait(false);
        await HelperMethods.Validate_OkResult(result).ConfigureAwait(false);
        var expectedPayerBalances = new Dictionary<string, int> { { addPointsTransaction.Payer, addPointsTransaction.Points } };
        Validate_PointsContext(context, 1, expectedPayerBalances );
    }

    /// <summary>
    /// Verifies that a single invalid transaction (would result in a negative balance) returns a failure as UnprocessableEntityObjectResult.
    /// </summary>
    /// <remarks>
    /// The in-memory <see cref="PointsContext"/> is fully recreated each time the method is run,
    /// meaning it starts with no records in any of its tables/collections.
    /// </remarks>
    [Fact]
    public async void Add_Individual_Negative_Balance_Returns_UnprocessableEntityObjectResult()
    {
        // ReSharper disable once StringLiteralTypo
        var             addPointsTransaction = new AddPointsTransaction("DANNON", -200, DateTime.Parse("2020-10-31T15:00:00Z"));
        await using var context              = HelperMethods.CreatePointsContext();
        var             addPointsCommand     = CreateAddPointsCommand(context);
        var             result               = await addPointsCommand.AddPoints(addPointsTransaction, CancellationToken.None).ConfigureAwait(false);
        await HelperMethods.Validate_UnprocessableEntityObjectResult(result, FetchRewardsApi.ErrorMessages.NegativeBalanceError).ConfigureAwait(false);
        var             expectedPayerBalances = new Dictionary<string, int>();

        Validate_PointsContext(context, 0, expectedPayerBalances );
    }

    /// <summary>
    /// Verifies that a sequence of transactions is added correctly.
    /// </summary>
    /// <param name="addPointsTransactions">A collection of transactions to add</param>
    /// <param name="expectedPayerBalances">The expected balance for each payer after all records have been processed</param>
    /// <remarks>
    /// The in-memory <see cref="PointsContext"/> is fully recreated each time the method is run,
    /// meaning it starts with no records in any of its tables/collections.
    /// </remarks>
    [Theory]
    [MemberData(nameof(TestData.SamplePointTransactions_Multiple_Successful), MemberType = typeof(TestData))]
    public async void Add_Many_Successful_Return_OkResult(IEnumerable<AddPointsTransaction> addPointsTransactions, IDictionary<string, int> expectedPayerBalances)
    {
        await using var context = HelperMethods.CreatePointsContext();
        var (recordsProcessed, failureCount) = await Process_All_AddPointsTransactions(context, addPointsTransactions).ConfigureAwait(false);
        const int expectedFailureCount = 0;
        Assert.Equal(expectedFailureCount, failureCount);
        Validate_PointsContext(context, recordsProcessed, expectedPayerBalances );
    }

    /// <summary>
    /// Verifies that a sequence of transactions which contains bad transactions adds good transactions correctly.
    /// </summary>
    /// <param name="addPointsTransactions">A collection of transactions to add</param>
    /// <param name="expectedFailureCount">The expected number of records in <paramref name="addPointsTransactions"/> which should be rejected.</param>
    /// <param name="expectedTransactionAndAvailableRecordCounts">
    /// The expected number of <see cref="PointTransaction"/> and <see cref="AvailablePoints"/> which should be added to the <see cref="PointsContext"/>.
    /// </param>
    /// <param name="expectedPayerBalances">The expected balances for each payer after all records have been processed</param>
    /// <remarks>
    /// The in-memory <see cref="PointsContext"/> is fully recreated each time the method is run,
    /// meaning it starts with no records in any of its tables/collections.
    /// </remarks>
    [Theory]
    [MemberData(nameof(TestData.SamplePointTransactions_Multiple_Some_Unsuccessful), MemberType = typeof(TestData))]
    public async void Add_Many_Some_Unsuccessful_Return_OkResult(IEnumerable<AddPointsTransaction> addPointsTransactions, int expectedFailureCount, int expectedTransactionAndAvailableRecordCounts, IDictionary<string, int> expectedPayerBalances)
    {
        await using var context = HelperMethods.CreatePointsContext();
        var (recordsProcessed, actualFailureCount) = await Process_All_AddPointsTransactions(context, addPointsTransactions).ConfigureAwait(false);
        Assert.Equal(expectedFailureCount, actualFailureCount );
        var actualTransactionAndAvailableRecordCounts = recordsProcessed - actualFailureCount;
        Assert.Equal(expectedTransactionAndAvailableRecordCounts, actualTransactionAndAvailableRecordCounts);
        Validate_PointsContext(context, expectedTransactionAndAvailableRecordCounts, expectedPayerBalances );
    }

    #region private helper methods
    /// <summary>
    /// Runs <see cref="IAddPoints.AddPoints"/> against a collection of <see cref="AddPointsTransaction"/> and returns a summary of the results.
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> used to get, add, and update entity records.</param>
    /// <param name="addPointsTransactions">The collection of <see cref="AddPointsTransaction"/> to be processed.</param>
    /// <returns>
    /// A <see cref="RecordsProcessedAndFailureCount"/> which summarizes the total number of records processed,
    /// the number of times <see cref="IAddPoints.AddPoints"/> failed, and the total expected amount added to each
    /// payer's <see cref="PayerBalance.Balance"/>.
    /// </returns>
    private static async Task<RecordsProcessedAndFailureCount> Process_All_AddPointsTransactions(PointsContext context, IEnumerable<AddPointsTransaction> addPointsTransactions)
    {
        var recordsProcessed = 0;
        var failureCount     = 0;
        
        foreach (var addPointsTransaction in addPointsTransactions)
        {
            recordsProcessed++;
            var addPointsCommand = CreateAddPointsCommand(context);
            var result           = await addPointsCommand.AddPoints(addPointsTransaction, CancellationToken.None).ConfigureAwait(false);
            var statusCode       = await HelperMethods.GetStatusCode(result).ConfigureAwait(false);
            if (statusCode == StatusCodes.Status200OK)
                continue;

            failureCount++;
            await HelperMethods.Validate_UnprocessableEntityObjectResult(result, FetchRewardsApi.ErrorMessages.NegativeBalanceError).ConfigureAwait(false);
        }

        return new RecordsProcessedAndFailureCount { RecordsProcessed = recordsProcessed, FailureCount = failureCount };
    }    
    
    /// <summary>
    /// Creates a new <see cref="AddPointsCommand"/>, the default implementation of <see cref="IAddPoints"/>.
    /// </summary>
    /// <param name="context">A <see cref="PointsContext"/> </param>
    /// <returns>A new, properly configured, <see cref="AddPointsCommand"/>. </returns>
    private static AddPointsCommand CreateAddPointsCommand(PointsContext context)
    {
        IPayerBalanceRepository     payerBalancesRepository    = new PayerBalanceRepository(context);
        IAdjustPayerPoints          adjustPayerPoints          = new AdjustPayerPointsCommand(payerBalancesRepository);
        IPointTransactionRepository pointTransactionRepository = new PointTransactionRepository(context);
        ICreatePointTransaction     createPointTransaction     = new CreatePointTransactionCommand(pointTransactionRepository);
        IAvailablePointsRepository  availableBalanceRepository = new AvailablePointsRepository(context);
        ICreateAvailablePoints      createAvailablePoints      = new CreateAvailablePointsCommand(availableBalanceRepository);
        var                         addPointsCommand           = new AddPointsCommand(context, adjustPayerPoints, createPointTransaction, createAvailablePoints);
        return addPointsCommand;
    }

    /// <summary>
    /// Verifies that tables which should not have records added to them were unaffected by <see cref="AddPointsCommand"/>
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> to be checked.</param>
    private static void Validate_Record_Count_Never_Affected_Tables(PointsContext context)
    {
        Assert.Empty(context.AllocatedPointsSet);
        Assert.Empty(context.SpentPointsSet);
    }

    /// <summary>
    /// Verifies that tables where records may have been added have the correct record counts. 
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> to be checked.</param>
    /// <param name="expectedTransactionAndAvailableRecordCounts">
    /// The expected number of records in tables <see cref="PointsContext.AvailablePointsSet"/> and <see cref="PointsContext.PointTransactions"/>.
    /// </param>
    /// <param name="expectedPayerBalanceCount"> the number of expected records in table <see cref="PointsContext.PayerBalances"/>. </param>
    private static void Validate_Record_Count_Affected_Tables(PointsContext context, int expectedTransactionAndAvailableRecordCounts, int expectedPayerBalanceCount)
    {
        Assert.Equal(expectedTransactionAndAvailableRecordCounts, context.AvailablePointsSet.Count());
        Assert.Equal(expectedTransactionAndAvailableRecordCounts, context.PointTransactions.Count());
        Assert.Equal(expectedPayerBalanceCount,                   context.PayerBalances.Count());
    }

    /// <summary>
    /// Validates that the total <see cref="PointTransaction.Points"/> for each payer has the correct values. 
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> which is being checked.</param>
    /// <param name="expectedPayerBalances">Each payer's expected balance.</param>
    private static void Validate_PointTransaction_Balances(PointsContext context, IDictionary<string, int> expectedPayerBalances)
    {
        foreach (var (key, expectedTotalPoints) in expectedPayerBalances)
        {
            var transactionPointsByPayerName = context.PointTransactions.Where(x => x.Payer == key).Sum(x => x.Points);
            Assert.Equal(expectedTotalPoints, transactionPointsByPayerName);

            var payerBalance = context.PayerBalances.SingleOrDefault(x => x.Payer == key);
            Assert.NotNull(payerBalance);

            var transactionPointsByNavigation = payerBalance.PointTransactions.Sum(x => x.Points);
            Assert.Equal(expectedTotalPoints,  transactionPointsByNavigation);
            Assert.Equal(payerBalance.Balance, transactionPointsByNavigation);
        }
    }

    /// <summary>
    /// Validates a <see cref="PointsContext"/> after all transactions have been processed.
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> to validate</param>
    /// <param name="expectedTransactionAndAvailableRecordCounts">
    /// The expected number of records in tables <see cref="PointsContext.AvailablePointsSet"/> and <see cref="PointsContext.PointTransactions"/>.
    /// </param>
    /// <param name="expectedPayerBalances">Each payer's expected balance.</param>
    private static void Validate_PointsContext(PointsContext context, int expectedTransactionAndAvailableRecordCounts, IDictionary<string, int> expectedPayerBalances)
    {
        Validate_Record_Count_Never_Affected_Tables(context);
        Validate_Record_Count_Affected_Tables(context, expectedTransactionAndAvailableRecordCounts, expectedPayerBalances.Count);
        HelperMethods.ValidatePayerBalances(context, expectedPayerBalances);
        Validate_PointTransaction_Balances(context, expectedPayerBalances);
    }
    #endregion
}