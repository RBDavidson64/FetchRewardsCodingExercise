using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;

// ReSharper disable InconsistentNaming

namespace UnitTests.TestHelpers;

/// <summary>
/// Records which can be passed to tests marked with <see cref="TheoryAttribute"/>
/// </summary>
internal static class TestData
{
    private static TheoryData<AddPointsTransaction>?                                                  _samplePointTransactions_Individual_Successful;
    private static TheoryData<IEnumerable<AddPointsTransaction>, IDictionary<string, int>>?           _samplePointTransactions_Multiple_Successful;
    private static TheoryData<IEnumerable<AddPointsTransaction>, int, int, IDictionary<string, int>>? _samplePointTransactions_Multiple_Some_Unsuccessful;
    
    /// <summary>
    /// Used by tests of <see cref="AddPointsCommand"/> where each record is added to a <see cref="PointsContext"/> which has no records.
    /// </summary>
    /// <returns>  A collection of <see cref="AddPointsTransaction"/> records where each one is valid by itself. </returns>
    /// <remarks>
    /// Transactions with a negative <see cref="AddPointsTransaction.Points"/> are excluded from this test as they
    /// would trigger the check which disallows a negative <see cref="PayerBalance.Balance"/>.
    /// </remarks>
    public static TheoryData<AddPointsTransaction> SamplePointTransactions_Individual_Successful()
    {
        if (_samplePointTransactions_Individual_Successful is not null) return _samplePointTransactions_Individual_Successful;

        _samplePointTransactions_Individual_Successful = new TheoryData<AddPointsTransaction>
                                                         {
                                                             // ReSharper disable StringLiteralTypo
                                                             new AddPointsTransaction (  "DANNON",      1000,  DateTime.Parse("2020-11-02T14:00:00Z") ),
                                                             new AddPointsTransaction ( "UNILEVER",     200,   DateTime.Parse("2020-10-31T11:00:00Z") ),
                                                             new AddPointsTransaction ( "MILLER COORS", 10000, DateTime.Parse("2020-11-01T14:00:00Z") ),
                                                             new AddPointsTransaction ( "DANNON",       300,   DateTime.Parse("2020-10-31T10:00:00Z"))
                                                             // ReSharper restore StringLiteralTypo
                                                         };

        return _samplePointTransactions_Individual_Successful;
    }

    /// <summary>
    /// Used by tests of <see cref="AddPointsCommand"/> where a sequence of <see cref="AddPointsTransaction"/> records are processed 
    /// and the sequence as a whole are valid.
    /// </summary>
    /// <returns> A collection of  <see cref="IEnumerable{AddPointsTransaction}"/> where all records should be processed correctly. </returns>
    /// <remarks>
    /// This test allows for some <see cref="AddPointsTransaction"/> records to have a negative <see cref="AddPointsTransaction.Points"/>
    /// so long as adding those negative points does not result in a negative <see cref="PayerBalance.Balance"/>. 
    /// </remarks>
    public static TheoryData<IEnumerable<AddPointsTransaction>, IDictionary<string, int>> SamplePointTransactions_Multiple_Successful()
    {
        if (_samplePointTransactions_Multiple_Successful is not null) return _samplePointTransactions_Multiple_Successful;
        _samplePointTransactions_Multiple_Successful = new TheoryData<IEnumerable<AddPointsTransaction>, IDictionary<string, int>>();

        // ReSharper disable StringLiteralTypo
        var transactions = new[] {
                                     new AddPointsTransaction (  "DANNON",      1000,  DateTime.Parse("2020-11-02T14:00:00Z") ),
                                     new AddPointsTransaction ( "UNILEVER",     200,   DateTime.Parse("2020-10-31T11:00:00Z") ),
                                     new AddPointsTransaction ( "DANNON",       -200,  DateTime.Parse("2020-10-31T15:00:00Z") ),
                                     new AddPointsTransaction ( "MILLER COORS", 10000, DateTime.Parse("2020-11-01T14:00:00Z") ),
                                     new AddPointsTransaction ( "DANNON",       300,   DateTime.Parse("2020-10-31T10:00:00Z"))
                                 };

        var expectedPayerBalances = new Dictionary<string, int> { { "DANNON", 1100 }, { "UNILEVER", 200 }, { "MILLER COORS", 10000 } };
        // ReSharper restore StringLiteralTypo
        _samplePointTransactions_Multiple_Successful.Add(transactions, expectedPayerBalances);

        // Subtract additional test cases below here.
        
        return _samplePointTransactions_Multiple_Successful;
    }
    
    /// <summary>
    /// Used by tests of <see cref="AddPointsCommand"/> where a sequence of <see cref="AddPointsTransaction"/> records include transactions are disallowed
    /// because they would result in a negative <see cref="PayerBalance.Balance"/>. All valid records should be processed correctly, while invalid records return the correct failure code and message. 
    /// </summary>
    /// <returns>
    /// An collection of  <see cref="IEnumerable{AddPointsTransaction}"/> where some records should be processed correctly and others rejected,
    /// </returns>
    public static TheoryData<IEnumerable<AddPointsTransaction>, int, int, IDictionary<string, int>> SamplePointTransactions_Multiple_Some_Unsuccessful()
    {
        if (_samplePointTransactions_Multiple_Some_Unsuccessful is not null) 
            return _samplePointTransactions_Multiple_Some_Unsuccessful;
        
        _samplePointTransactions_Multiple_Some_Unsuccessful = new TheoryData<IEnumerable<AddPointsTransaction>, int, int, IDictionary<string, int>>();
        // ReSharper disable StringLiteralTypo
        var transactions = new[] {
                                     new AddPointsTransaction (  "DANNON",      1000,  DateTime.Parse("2020-11-02T14:00:00Z") ),
                                     new AddPointsTransaction ( "UNILEVER",     200,   DateTime.Parse("2020-10-31T11:00:00Z") ),
                                     new AddPointsTransaction ( "DANNON",       -2000, DateTime.Parse("2020-10-31T15:00:00Z") ),
                                     new AddPointsTransaction ( "MILLER COORS", 10000, DateTime.Parse("2020-11-01T14:00:00Z") ),
                                     new AddPointsTransaction ( "DANNON",       300,   DateTime.Parse("2020-10-31T10:00:00Z"))
                                     
                                 };

        var expectedPayerBalances = new Dictionary<string, int> { { "DANNON", 1300 }, { "UNILEVER", 200 }, { "MILLER COORS", 10000 } };
        // ReSharper restore StringLiteralTypo
        const int expectedFailureCount                        = 1;
        const int expectedTransactionAndAvailableRecordCounts = 4;
        _samplePointTransactions_Multiple_Some_Unsuccessful.Add(transactions, expectedFailureCount, expectedTransactionAndAvailableRecordCounts, expectedPayerBalances);
        
        // Subtract other test collections and expected counts below here.

        return _samplePointTransactions_Multiple_Some_Unsuccessful;
    }
    
    /// <summary>
    /// Used to test the <see cref="GetBalancesCommand"/>.
    /// </summary>
    /// <returns>Dictionaries of payer balances.</returns>
    public static TheoryData<IDictionary<string, int>> PayerBalances()
    {
        // ReSharper disable once StringLiteralTypo
        var expectedPayerBalances = new Dictionary<string, int> { { "DANNON", 1300 }, { "UNILEVER", 200 }, { "MILLER COORS", 10000 } };
        var payerBalances               = new TheoryData<IDictionary<string, int>> { expectedPayerBalances };
        return payerBalances;
    }

    /// <summary>
    /// Used to test the <see cref="SpendPointsCommand"/> when the <see cref="SpendPointsTransaction.Points"/> is greater than
    /// the sum of all <see cref="PayerBalance.Balance"/> values.
    /// </summary>
    /// <returns>A dictionary of payer balances and an <see cref="int"/> sum on their total points.</returns>
    public static TheoryData<IDictionary<string, int>, int> PayerBalances_And_TotalPoints()
    {
        // ReSharper disable once StringLiteralTypo
        var expectedPayerBalances = new Dictionary<string, int> { { "DANNON", 1300 }, { "UNILEVER", 200 }, { "MILLER COORS", 10000 } };
        var payerBalances         = new TheoryData<IDictionary<string, int>, int>();
        var totalPoints           = expectedPayerBalances.Values.Sum();
        payerBalances.Add(expectedPayerBalances, totalPoints);
        return payerBalances;
    }

    /// <summary>
    /// Used to the test the <see cref="SpendPointsCommand"/> when it executes successfully.
    /// </summary>
    /// <returns>
    /// Records representing the initial state of the context, the <see cref="SpendPointsTransaction"/>, the expected results from
    /// the <see cref="SpendPointsCommand"/>, and the expected final state of the <see cref="PointsContext"/> 
    /// </returns>
    public static TheoryData<ContextStateBeforeRunningSpendPointsCommand, ICollection<SpendPointTransactionAndExpectedResults>, ExpectedFinalContextState> ContextState_SpendPointTransactionAndExpectedResults_ExpectedFinalContextState_Successful()
    {
        var result = new TheoryData<ContextStateBeforeRunningSpendPointsCommand, ICollection<SpendPointTransactionAndExpectedResults>, ExpectedFinalContextState >();

        var (contextState1, spendPointTransactionAndExpectedResultsCollection1, expectedResults1) = CreateSpendPointCommandTestValues_Test1();
        result.Add(contextState1, spendPointTransactionAndExpectedResultsCollection1, expectedResults1);

        var (contextState2, spendPointTransactionAndExpectedResultsCollection2, expectedResults2) = CreateSpendPointCommandTestValues_Test2();
        result.Add(contextState2, spendPointTransactionAndExpectedResultsCollection2, expectedResults2);

        return result;
    }

    /// <summary>
    /// Creates the values for running the first test of <see cref="SpendPointsCommand"/>
    /// </summary>
    /// <returns>A <see cref="Tuple"/> with a <see cref="ContextStateBeforeRunningSpendPointsCommand"/>, a collection of <see cref="SpendPointTransactionAndExpectedResults"/>, and a <see cref="ExpectedFinalContextState"/> </returns>
    private static (ContextStateBeforeRunningSpendPointsCommand contextState, SpendPointTransactionAndExpectedResults[] spendPointTransactionAndExpectedResultsCollection, ExpectedFinalContextState expectedResults) CreateSpendPointCommandTestValues_Test1()
    {
        // ReSharper disable once StringLiteralTypo
        // ReSharper disable once IdentifierTypo
        const string dannon      = "DANNON";
        const string unilever    = "UNILEVER";
        const string millerCoors = "MILLER COORS";

        var payerBalances = new[]
                            {
                                new PayerBalance
                                {
                                    PayerBalanceId    = 1,
                                    Payer             = dannon,
                                    Balance           = 1100,
                                    AvailablePoints   = new List<AvailablePoints>(),
                                    PointTransactions = new List<PointTransaction>()
                                },
                                new PayerBalance
                                {
                                    PayerBalanceId    = 2,
                                    Payer             = unilever,
                                    Balance           = 200,
                                    AvailablePoints   = new List<AvailablePoints>(),
                                    PointTransactions = new List<PointTransaction>()
                                },
                                new PayerBalance
                                {
                                    PayerBalanceId    = 3,
                                    Payer             = millerCoors,
                                    Balance           = 10000,
                                    AvailablePoints   = new List<AvailablePoints>(),
                                    PointTransactions = new List<PointTransaction>()
                                }
                            };

        var payerBalanceDictionary = payerBalances.ToDictionary(x => x.Payer);

        var pointTransactions = new[] { CreatePointTransaction(payerBalanceDictionary[dannon], 1, 1000, DateTime.Parse("2020-11-02T14:00:00Z")), CreatePointTransaction(payerBalanceDictionary[unilever], 2, 200, DateTime.Parse("2020-10-31T11:00:00Z")), CreatePointTransaction(payerBalanceDictionary[dannon], 3, -200, DateTime.Parse("2020-10-31T15:00:00Z")), CreatePointTransaction(payerBalanceDictionary[millerCoors], 4, 10000, DateTime.Parse("2020-11-01T14:00:00Z")), CreatePointTransaction(payerBalanceDictionary[dannon], 5, 300, DateTime.Parse("2020-10-31T10:00:00Z")) };

        var availablePoints = pointTransactions.Select(CreateAvailablePoints).ToArray();

        var contextState = new ContextStateBeforeRunningSpendPointsCommand(pointTransactions, availablePoints, payerBalances);

        // Note: the "with" in the Select statement will create new instances of PointTransaction.   
        var expectedPointTransactions = pointTransactions.Select(x => x with { AllocatedPoints = new List<AllocatedPoints>(4) }).ToArray();

        var expectedAvailablePoints = new[] { availablePoints[0] with { PointTransaction = expectedPointTransactions[0] }, availablePoints[1] with { PointTransaction = expectedPointTransactions[1], AllocatedPoints = 200, UnallocatedPoints = 0, AllPointsAllocated = true }, availablePoints[2] with { PointTransaction = expectedPointTransactions[2], AllocatedPoints = -200, UnallocatedPoints = 0, AllPointsAllocated = true }, availablePoints[3] with { PointTransaction = expectedPointTransactions[3], AllocatedPoints = 4700, UnallocatedPoints = 5300 }, availablePoints[4] with { PointTransaction = expectedPointTransactions[4], AllocatedPoints = 300, UnallocatedPoints = 0, AllPointsAllocated = true } };

        var spendPointsTransaction = new SpendPointsTransaction(5000);

        var expectedSpentPoints = new[] { new SpentPoints { SpentPointsId = 1, PointsSpent = spendPointsTransaction.Points, TimeStamp = DateTime.Now, AllocatedPoints = new List<AllocatedPoints>(4) } };

        ICollection<AllocatedPoints> expectedAllocatedPoints = new[] { CreateAllocatedPoints(1, expectedAvailablePoints[4], expectedSpentPoints[0]), CreateAllocatedPoints(2, expectedAvailablePoints[1], expectedSpentPoints[0]), CreateAllocatedPoints(3, expectedAvailablePoints[2], expectedSpentPoints[0]), CreateAllocatedPoints(4, expectedAvailablePoints[3], expectedSpentPoints[0]), };

        var spendPointsTransactionResults = expectedAllocatedPoints
                                           .GroupBy(x => x.Payer)
                                           .ToDictionary(
                                                         x => x.Key,
                                                         x => -x.Sum(a => a.PointsAllocated)
                                                        );

        var spendPointTransactionAndExpectedResults           = new SpendPointTransactionAndExpectedResults(spendPointsTransaction, spendPointsTransactionResults);
        var spendPointTransactionAndExpectedResultsCollection = new[] { spendPointTransactionAndExpectedResults };
        var expectedPayerBalances                             = new[] { CreateExpectedPayerBalance(payerBalanceDictionary[dannon], 1000, expectedPointTransactions, expectedAvailablePoints, expectedAllocatedPoints), CreateExpectedPayerBalance(payerBalanceDictionary[unilever], 0, expectedPointTransactions, expectedAvailablePoints, expectedAllocatedPoints), CreateExpectedPayerBalance(payerBalanceDictionary[millerCoors], 5300, expectedPointTransactions, expectedAvailablePoints, expectedAllocatedPoints) };

        var expectedResults = new ExpectedFinalContextState(
                                                            expectedAllocatedPoints,
                                                            expectedAvailablePoints,
                                                            expectedPayerBalances,
                                                            expectedAvailablePoints.Select(x => x.PointTransaction).ToArray(),
                                                            expectedSpentPoints
                                                           );

        return (contextState, spendPointTransactionAndExpectedResultsCollection, expectedResults);
    }

       /// <summary>
    /// Creates the values for running the second test of <see cref="SpendPointsCommand"/>
    /// </summary>
    /// <returns>A <see cref="Tuple"/> with a <see cref="ContextStateBeforeRunningSpendPointsCommand"/>, a collection of <see cref="SpendPointTransactionAndExpectedResults"/>, and a <see cref="ExpectedFinalContextState"/> </returns>
    private static (ContextStateBeforeRunningSpendPointsCommand contextState, SpendPointTransactionAndExpectedResults[] spendPointTransactionAndExpectedResultsCollection, ExpectedFinalContextState expectedResults) CreateSpendPointCommandTestValues_Test2()
    {
        // ReSharper disable once StringLiteralTypo
        // ReSharper disable once IdentifierTypo
        const string dannon      = "DANNON";
        const string unilever    = "UNILEVER";
        const string millerCoors = "MILLER COORS";

        var payerBalances = new[]
                            {
                                new PayerBalance
                                {
                                    PayerBalanceId    = 1,
                                    Payer             = dannon,
                                    Balance           = 100,
                                    AvailablePoints   = new List<AvailablePoints>(),
                                    PointTransactions = new List<PointTransaction>()
                                },
                                new PayerBalance
                                {
                                    PayerBalanceId    = 2,
                                    Payer             = unilever,
                                    Balance           = 200,
                                    AvailablePoints   = new List<AvailablePoints>(),
                                    PointTransactions = new List<PointTransaction>()
                                },
                                new PayerBalance
                                {
                                    PayerBalanceId    = 3,
                                    Payer             = millerCoors,
                                    Balance           = 10000,
                                    AvailablePoints   = new List<AvailablePoints>(),
                                    PointTransactions = new List<PointTransaction>()
                                }
                            };

        var payerBalanceDictionary = payerBalances.ToDictionary(x => x.Payer);

        var pointTransactions = new[]
                                {
                                    CreatePointTransaction(payerBalanceDictionary[dannon],      1, 300,   DateTime.Parse("2020-10-31T10:00:00Z")),
                                    CreatePointTransaction(payerBalanceDictionary[unilever],    2, 200,   DateTime.Parse("2020-10-31T11:00:00Z")), 
                                    CreatePointTransaction(payerBalanceDictionary[dannon],      3, -200,  DateTime.Parse("2020-10-31T15:00:00Z")), 
                                    CreatePointTransaction(payerBalanceDictionary[millerCoors], 4, 10000, DateTime.Parse("2020-11-01T14:00:00Z")) 
                                };

        var availablePoints = pointTransactions.Select(CreateAvailablePoints).ToArray();

        var contextState = new ContextStateBeforeRunningSpendPointsCommand(pointTransactions, availablePoints, payerBalances);

        // Note: the "with" in the Select statement will create new instances of PointTransaction.   
        var expectedPointTransactions = pointTransactions.Select(x => x with { AllocatedPoints = new List<AllocatedPoints>(4) }).ToArray();

        var expectedAvailablePoints = new[]
                                      {
                                          availablePoints[0] with { PointTransaction = expectedPointTransactions[0], AllocatedPoints = 300, UnallocatedPoints = 0, AllPointsAllocated = true}, 
                                          availablePoints[1] with { PointTransaction = expectedPointTransactions[1], AllocatedPoints = 200, UnallocatedPoints = 0, AllPointsAllocated = true }, 
                                          availablePoints[2] with { PointTransaction = expectedPointTransactions[2], AllocatedPoints = -200, UnallocatedPoints = 0, AllPointsAllocated = true }, 
                                          availablePoints[3] with { PointTransaction = expectedPointTransactions[3], AllocatedPoints = 4700, UnallocatedPoints = 5300 }
                                      };

        var spendPointsTransaction = new SpendPointsTransaction(5000);

        var expectedSpentPoints = new[] { new SpentPoints { SpentPointsId = 1, PointsSpent = spendPointsTransaction.Points, TimeStamp   = DateTime.Now, AllocatedPoints = new List<AllocatedPoints>(4) } };

        ICollection<AllocatedPoints> expectedAllocatedPoints = new[]
                                                               {
                                                                   CreateAllocatedPoints(1, expectedAvailablePoints[0], expectedSpentPoints[0]), 
                                                                   CreateAllocatedPoints(2, expectedAvailablePoints[1], expectedSpentPoints[0]), 
                                                                   CreateAllocatedPoints(3, expectedAvailablePoints[2], expectedSpentPoints[0]), 
                                                                   CreateAllocatedPoints(4, expectedAvailablePoints[3], expectedSpentPoints[0]),
                                                               };

        var spendPointsTransactionResults = expectedAllocatedPoints
                                           .GroupBy(x => x.Payer)
                                           .ToDictionary(
                                                         x => x.Key,
                                                         x => -x.Sum(a => a.PointsAllocated)
                                                        );

        var spendPointTransactionAndExpectedResults           = new SpendPointTransactionAndExpectedResults(spendPointsTransaction, spendPointsTransactionResults);
        var spendPointTransactionAndExpectedResultsCollection = new[] { spendPointTransactionAndExpectedResults };
        var expectedPayerBalances                             = new[]
                                                                {
                                                                    CreateExpectedPayerBalance(payerBalanceDictionary[dannon], 0, expectedPointTransactions, expectedAvailablePoints, expectedAllocatedPoints), 
                                                                    CreateExpectedPayerBalance(payerBalanceDictionary[unilever], 0, expectedPointTransactions, expectedAvailablePoints, expectedAllocatedPoints), 
                                                                    CreateExpectedPayerBalance(payerBalanceDictionary[millerCoors], 5300, expectedPointTransactions, expectedAvailablePoints, expectedAllocatedPoints)
                                                                };

        var expectedResults = new ExpectedFinalContextState(
                                                            expectedAllocatedPoints,
                                                            expectedAvailablePoints,
                                                            expectedPayerBalances,
                                                            expectedAvailablePoints.Select(x => x.PointTransaction).ToArray(),
                                                            expectedSpentPoints
                                                           );

        return (contextState, spendPointTransactionAndExpectedResultsCollection, expectedResults);
    }

    /// <summary>
    /// A private method used to construct an expected <see cref="PayerBalance"/> record after a <see cref="SpendPointsCommand"/> has executed.
    /// </summary>
    /// <param name="originalPayerBalance">The <see cref="PayerBalance"/> record before the <see cref="SpendPointsCommand"/> executed.</param>
    /// <param name="newBalance">The payer's balance after the <see cref="SpendPointsCommand"/> completes.</param>
    /// <param name="expectedPointTransactions">The expected <see cref="PayerBalance.PointTransactions"/> records </param>
    /// <param name="expectedAvailablePoints">The expected <see cref="PayerBalance.AvailablePoints"/> records</param>
    /// <param name="expectedAllocatedPoints">The expected <see cref="PayerBalance.AllocatedPoints"/> records</param>
    /// <returns></returns>
    private static PayerBalance CreateExpectedPayerBalance(PayerBalance originalPayerBalance, int newBalance, IEnumerable<PointTransaction> expectedPointTransactions, IEnumerable<AvailablePoints> expectedAvailablePoints, IEnumerable<AllocatedPoints> expectedAllocatedPoints)
    {
        var pointTransactions = expectedPointTransactions.Where(x => x.Payer == originalPayerBalance.Payer).ToList();
        var availablePoints   = expectedAvailablePoints.Where(x => x.Payer   == originalPayerBalance.Payer).ToList();
        var allocatedPoints   = expectedAllocatedPoints.Where(x => x.Payer   == originalPayerBalance.Payer).ToList();

        var payer = originalPayerBalance with { Balance = newBalance, PointTransactions = pointTransactions, AvailablePoints = availablePoints, AllocatedPoints = allocatedPoints };
        return payer;
    }

    /// <summary>
    /// Creates a <see cref="PointTransaction"/> record before the <see cref="SpendPointsCommand"/> executes
    /// </summary>
    /// <param name="payerBalance">The <see cref="PointTransaction.PayerBalance"/></param>
    /// <param name="id">The <see cref="PointTransaction.PointTransactionId"/></param>
    /// <param name="points">The <see cref="PointTransaction.Points"/></param>
    /// <param name="timeStamp">The <see cref="PointTransaction.TimeStamp"/></param>
    /// <returns>A new <see cref="PointTransaction"/> record with correct values.</returns>
    private static PointTransaction CreatePointTransaction(PayerBalance payerBalance, int id, int points, DateTime timeStamp)
    {
        var pointTransaction = new PointTransaction
                 {
                     Payer = payerBalance.Payer,
                     PayerBalance = payerBalance,
                     Points = points,
                     PointTransactionId = id,
                     TimeStamp = timeStamp
                 };
        payerBalance.PointTransactions.Add(pointTransaction);
        return pointTransaction;
    }

    /// <summary>
    /// Create an expected <see cref="AllocatedPoints"/> record to validate against after the <see cref="SpendPointsCommand"/> has completed.
    /// </summary>
    /// <param name="id">The <see cref="AllocatedPoints.AllocatedPointsId"/></param>
    /// <param name="availablePoint">The <see cref="AllocatedPoints.AvailablePoints"/></param>
    /// <param name="spentPoints">The <see cref="AllocatedPoints.SpentPoints"/></param>
    /// <returns>A new <see cref="AllocatedPoints"/> record used during validation of a <see cref="SpendPointsCommand"/></returns>
    private static AllocatedPoints CreateAllocatedPoints(int id, AvailablePoints availablePoint, SpentPoints spentPoints)
    {
        var allocatedPoints = new AllocatedPoints
               {
                   AllocatedPointsId = id,
                   AvailablePoints  = availablePoint,
                   Payer            = availablePoint.Payer,
                   PayerBalance     = availablePoint.PayerBalance,
                   PointTransaction = availablePoint.PointTransaction,
                   SpentPoints      = spentPoints,
                   PointsAllocated  = availablePoint.AllocatedPoints,
                   TimeStamp = DateTime.Now
               };
        
        availablePoint.PointTransaction.AllocatedPoints.Add(allocatedPoints);
        spentPoints.AllocatedPoints.Add(allocatedPoints);
        return allocatedPoints;
    }

    /// <summary>
    /// Creates a new <see cref="AvailablePoints"/> entity record to assign to the <see cref="PointsContext"/> before a <see cref="SpendPointsCommand"/> is executed.
    /// </summary>
    /// <param name="pointTransaction">A <see cref="PointTransaction"/> entity record. Used to create the new <see cref="AvailablePoints"/> entity record and is assigned to <see cref="AvailablePoints.PointTransaction"/> </param>
    /// <returns>A new <see cref="AvailablePoints"/> entity record.</returns>
    private static AvailablePoints CreateAvailablePoints(PointTransaction pointTransaction)
    {
        var availablePoints = new AvailablePoints
                {
                    AllocatedPoints           = 0,
                    AllPointsAllocated = false,
                    AvailablePointsId         = pointTransaction.PointTransactionId,
                    OriginalUnallocatedPoints = pointTransaction.Points,
                    Payer                     = pointTransaction.Payer,
                    PayerBalance              = pointTransaction.PayerBalance,
                    PointTransaction          = pointTransaction,
                    PointTransactionTimeStamp = pointTransaction.TimeStamp,
                    PointTransactionId = pointTransaction.PointTransactionId,
                    UnallocatedPoints = pointTransaction.Points
                };

        pointTransaction.AvailablePoints = availablePoints;
        pointTransaction.PayerBalance.AvailablePoints.Add(availablePoints);
        return availablePoints;
    }
}