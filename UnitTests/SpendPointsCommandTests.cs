using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTests.TestHelpers;

namespace UnitTests;

/// <summary>
/// Validates the <see cref="SpendPointsCommand"/>
/// </summary>
public sealed class SpendPointsCommandTests
{
    /// <summary>
    /// Validate <see cref="SpendPointsCommand"/> rejects a <see cref="SpendPointsTransaction"/> with negative <see cref="SpendPointsTransaction.Points"/>.
    /// Should return a <see cref="StatusCodes.Status422UnprocessableEntity"/> and a <see cref="ProblemDetails"/> containing
    /// a <see cref="FetchRewardsApi.ErrorMessages.NegativeSpendPointsError"/> message.
    /// </summary>
    [Fact]
    public async Task Spending_Negative_Points_Returns_UnprocessableEntityObjectResult()
    {
        await using var context                = HelperMethods.CreatePointsContext();
        var             spendPointsCommand     = CreateSpendPointsCommand(context);
        var             spendPointsTransaction = new SpendPointsTransaction(-10);
        var             result                 = await spendPointsCommand.SpendPoints(spendPointsTransaction, CancellationToken.None);
        await HelperMethods.Validate_UnprocessableEntityObjectResult(result, FetchRewardsApi.ErrorMessages.NegativeSpendPointsError).ConfigureAwait(false);
    }

    /// <summary>
    /// Validate <see cref="SpendPointsCommand"/> rejects a <see cref="SpendPointsTransaction"/> with
    /// more <see cref="SpendPointsTransaction.Points"/> than the sum of all <see cref="PayerBalance.Balance"/>.
    /// Should return a <see cref="StatusCodes.Status422UnprocessableEntity"/> and a <see cref="ProblemDetails"/> containing
    /// a <see cref="FetchRewardsApi.ErrorMessages.SpendingMorePointsThanAvailableError"/> message.
    /// </summary>
    /// <param name="payerBalances">The payer balances to add to the <see cref="PointsContext"/> before the <see cref="SpendPointsCommand"/> is executed.</param>
    /// <param name="totalPoints">The total of all <see cref="PayerBalance.Balance"/></param>
    [Theory]
    [MemberData(nameof(TestData.PayerBalances_And_TotalPoints), MemberType = typeof(TestData))]
    public async Task Spending_More_Points_Than_Available_Returns_UnprocessableEntityObjectResult(IDictionary<string, int> payerBalances, int totalPoints)
    {
        await using var context            = HelperMethods.CreatePointsContext();
        var             spendPointsCommand = CreateSpendPointsCommand(context);
        AddPayerBalances(context, payerBalances);
        var             spendPointsTransaction = new SpendPointsTransaction(totalPoints + 1);
        var             result                 = await spendPointsCommand.SpendPoints(spendPointsTransaction, CancellationToken.None);
        await HelperMethods.Validate_UnprocessableEntityObjectResult(result, FetchRewardsApi.ErrorMessages.SpendingMorePointsThanAvailableError).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates a successful execution of the <see cref="SpendPointsCommand"/>
    /// </summary>
    /// <param name="initialContextStateBeforeRunning">All entity records to add to the <see cref="PointsContext"/> before the <see cref="SpendPointsCommand"/> is executed </param>
    /// <param name="spendPointTransactionsAndExpectedResults">The <see cref="SpendPointsTransaction"/> and the expected JSON results.</param>
    /// <param name="expectedFinalContextState">The expected state entity records in the <see cref="PointsContext"/> after the <see cref="SpendPointsCommand"/> completes.</param>
    [Theory]
    [MemberData(nameof(TestData.ContextState_SpendPointTransactionAndExpectedResults_ExpectedFinalContextState_Successful), MemberType = typeof(TestData))]
    internal async Task SpendPoints_Successful(ContextStateBeforeRunningSpendPointsCommand initialContextStateBeforeRunning, IEnumerable<SpendPointTransactionAndExpectedResults> spendPointTransactionsAndExpectedResults, ExpectedFinalContextState expectedFinalContextState)
    {
        await using var context            = HelperMethods.CreatePointsContext();
        
        initialContextStateBeforeRunning.ConfigureContextState(context);

        foreach (var (spendPointsTransaction, expectedResultsByPayer) in spendPointTransactionsAndExpectedResults)
        {
            var             spendPointsCommand = CreateSpendPointsCommand(context);
            var             result             = await spendPointsCommand.SpendPoints(spendPointsTransaction, CancellationToken.None);
            await Validate_JsonResult(result, expectedResultsByPayer).ConfigureAwait(false);
        }

        expectedFinalContextState.ValidateFinalContextState(context);
    }

    #region private helper methods
    /// <summary>
    /// Creates a new <see cref="SpendPointsCommand"/>
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> which the <see cref="SpendPointsCommand"/> will be executed against.</param>
    /// <returns>A new <see cref="SpendPointsCommand"/></returns>
    private static SpendPointsCommand CreateSpendPointsCommand(PointsContext context)
    {
        IPayerBalanceRepository    payerBalanceRepository    = new PayerBalanceRepository(context);
        ISpentPointsRepository     spentPointsRepository     = new SpentPointsRepository(context);
        IAvailablePointsRepository availablePointsRepository = new AvailablePointsRepository(context);
        IAllocatedPointsRepository allocatedPointsRepository = new AllocatedPointsRepository(context);
        ICreateSpentPoints         createSpentPoints         = new CreateSpentPointsCommand(payerBalanceRepository, spentPointsRepository);
        ISpendPointsResults        spendPointsResults        = new SpendPointsResults();
        IAllocateAvailablePoints   allocateAvailablePoints   = new AllocateAvailablePointsCommand(allocatedPointsRepository, availablePointsRepository, payerBalanceRepository);
        IAllocatePoints            allocatePoints            = new AllocatePointsCommand(availablePointsRepository, allocateAvailablePoints, spendPointsResults, payerBalanceRepository);
        var                        spendPointsCommand        = new SpendPointsCommand(context, createSpentPoints, allocatePoints);
        return spendPointsCommand;
    }

    /// <summary>
    /// Initializes the <see cref="PointsContext.PayerBalances"/> entity records 
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> which will have records added to it.</param>
    /// <param name="payerBalances">The payers and their balances</param>
    private static void AddPayerBalances(PointsContext context, IDictionary<string, int> payerBalances)
    {
        var       payerBalancesRecords = payerBalances.Select(x => new PayerBalance { Payer = x.Key, Balance = x.Value });
        context.PayerBalances.AddRange(payerBalancesRecords);
        context.SaveChanges();
    }

    /// <summary>
    /// Validate the <see cref="IResult"/> returned by the <see cref="SpendPointsCommand"/>
    /// </summary>
    /// <param name="result">The <see cref="IResult"/> to validate</param>
    /// <param name="expectedResultsByPayer">The expected net changes to payer balances reported by the <see cref="SpendPointsCommand"/></param>
    private static async Task Validate_JsonResult(IResult? result, IDictionary<string, int> expectedResultsByPayer)
    {
        Assert.NotNull(result);
        var (spendPointsResults, statusCode) = await HelperMethods.GetResponseValue<SpendPointsResult[]>(result).ConfigureAwait(false);
        
        Assert.Equal(StatusCodes.Status200OK, statusCode);
        Assert.NotNull(spendPointsResults);
        Assert.Equal(expectedResultsByPayer.Count, spendPointsResults.Length);
        
        foreach (var (payer, expectedPoints) in expectedResultsByPayer)
        {
            var actual = spendPointsResults.SingleOrDefault(x => x.Payer == payer);
            Assert.NotNull(actual);
            Assert.Equal(expectedPoints, actual.Points);
        }
    }
    #endregion
}