using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Repositories;
using Microsoft.AspNetCore.Http;
using UnitTests.TestHelpers;

namespace UnitTests;

/// <summary>
/// Validates the <see cref="GetBalancesCommand"/>
/// </summary>
public sealed class GetBalancesTest
{
    /// <summary>
    /// Validates the <see cref="GetBalancesCommand"/> against an empty <see cref="PointsContext"/>
    /// </summary>
    [Fact]
    public async Task GetBalances_Empty_Context()
    {
        var         context                = HelperMethods.CreatePointsContext();
        await using var         _                     = context.ConfigureAwait(false);
        IPayerBalanceRepository payerBalanceRepository = new PayerBalanceRepository(context);
        var                     getBalanceCommand      = new GetBalancesCommand(payerBalanceRepository);
        var                     result                 = await getBalanceCommand.GetBalances(CancellationToken.None).ConfigureAwait(false);
        IDictionary<string,int> expectedPayerBalances  = new Dictionary<string, int>( );
        await Validate_JsonResult(result, expectedPayerBalances).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the <see cref="GetBalancesCommand"/> against a <see cref="PointsContext"/> with a single <see cref="PayerBalance"/> record.
    /// </summary>
    /// <param name="addPointsTransaction">The transaction with the payer information to add to the <see cref="PointsContext"/></param>
    /// <remarks>
    /// The in-memory <see cref="PointsContext"/> is fully recreated each time the method is run,
    /// meaning it starts with no records in any of its tables/collections.
    /// </remarks>
    [Theory]
    [MemberData(nameof(TestData.SamplePointTransactions_Individual_Successful), MemberType = typeof(TestData))]
    public async Task GetBalances_Single_Payer(AddPointsTransaction addPointsTransaction)
    {
        IDictionary<string,int> expectedPayerBalances  = new Dictionary<string, int>{{addPointsTransaction.Payer, addPointsTransaction.Points}};
        var                     context                = HelperMethods.CreatePointsContext();
        await using var         _                      = context.ConfigureAwait(false);
        IPayerBalanceRepository payerBalanceRepository = new PayerBalanceRepository(context);
        AddPayers(context, expectedPayerBalances);
        var getBalanceCommand = new GetBalancesCommand(payerBalanceRepository);
        var result            = await getBalanceCommand.GetBalances(CancellationToken.None).ConfigureAwait(false);
        await Validate_JsonResult(result, expectedPayerBalances).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Validates the <see cref="GetBalancesCommand"/> against a <see cref="PointsContext"/> with multiple <see cref="PayerBalance"/> entity records. 
    /// </summary>
    /// <param name="expectedPayerBalances">The expected balance for each payer</param>
    [Theory]
    [MemberData(nameof(TestData.PayerBalances), MemberType = typeof(TestData))]
    public async void Add_Many_Successful_Return_OkResult(IDictionary<string, int> expectedPayerBalances)
    {
        var         context                = HelperMethods.CreatePointsContext();
        await using var         _                     = context.ConfigureAwait(false);
        IPayerBalanceRepository payerBalanceRepository = new PayerBalanceRepository(context);
        AddPayers(context, expectedPayerBalances);
        var getBalanceCommand = new GetBalancesCommand(payerBalanceRepository);
        var result            = await getBalanceCommand.GetBalances(CancellationToken.None).ConfigureAwait(false);
        await Validate_JsonResult(result, expectedPayerBalances).ConfigureAwait(false);
    }

    #region private helper methods
    /// <summary>
    /// Add <see cref="PayerBalance"/> records to a <see cref="PointsContext"/>
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> to have records added to it.</param>
    /// <param name="expectedPayerBalances">The payers and their balances.</param>
    private static void AddPayers(PointsContext context, IDictionary<string, int> expectedPayerBalances)
    {
        var       payerBalances = expectedPayerBalances.Select(x => new PayerBalance { Payer = x.Key, Balance = x.Value });
        context.PayerBalances.AddRange(payerBalances);
        context.SaveChanges();
    }

    /// <summary>
    /// Validate the <see cref="GetBalancesCommand"/> returned an <see cref="Results.Ok"/> containing the expected JSON <see cref="Balance"/> api records. 
    /// </summary>
    /// <param name="result">The <see cref="IResult"/> to validate</param>
    /// <param name="expectedPayerBalances">The expected <see cref="Balance"/> api records returned.</param>
    private static async Task Validate_JsonResult(IResult? result, IDictionary<string, int> expectedPayerBalances)
    {
        Assert.NotNull(result);
        var (actualBalances, statusCode) = await HelperMethods.GetResponseValue<ICollection<Balance>>(result).ConfigureAwait(false);
        Assert.NotNull(actualBalances);
        Assert.IsAssignableFrom<ICollection<Balance>>(actualBalances);
        Assert.Equal(200,                         statusCode);
        Assert.Equal(expectedPayerBalances.Count, actualBalances.Count);

        foreach (var (payer, expectedBalance) in expectedPayerBalances)
        {
            var actual = actualBalances.SingleOrDefault(x => x.Payer == payer);
            Assert.NotNull(actual);
            Assert.Equal(expectedBalance, actual.Points);
        }
    }
    
    #endregion
}