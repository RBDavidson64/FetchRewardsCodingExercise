using FetchRewardsApi.EntityModel;
using Microsoft.EntityFrameworkCore;
using UnitTests.TestHelpers;

namespace UnitTests;

public sealed class PointTransactionTests
{
    // [Fact]
    // public void GetPointTransactions()
    // {
    //     using var context           = HelperMethods.CreatePointsContext();
    //     var       pointTransactions = context.PointTransactions;
    //     Assert.NotNull(pointTransactions);
    //     Assert.Empty(pointTransactions);
    // }
    //
    // [Fact]
    // public void SaveNewPointTransaction()
    // {
    //     using var    context          = HelperMethods.CreatePointsContext();
    //     const string payer            = "SomePayer";
    //     var          pointTransaction = new PointTransaction { Payer = payer };
    //     context.Add(pointTransaction);
    //     const int expected = 1;
    //     Assert.Equal(expected, context.SaveChanges());
    // }
    //
    // [Fact]
    // public void SaveNewPointTransaction_AutoIncrementKey()
    // {
    //     using var context                   = HelperMethods.CreatePointsContext();
    //     Assert.Empty(context.PointTransactions);
    //     const string payer                     = "SomePayer";
    //     var          pointTransaction          = new PointTransaction { Payer = payer };
    //     const int    expectedValueBeforeSaving = 0;
    //     Assert.Equal(expectedValueBeforeSaving, pointTransaction.PointTransactionId);
    //     context.Add(pointTransaction);
    //     context.SaveChanges();
    //     Assert.NotEmpty(context.PointTransactions);
    //     const int expectedValueAfterSaving = 1;
    //     Assert.NotEqual(expectedValueBeforeSaving, pointTransaction.PointTransactionId);
    //     Assert.Equal(expectedValueAfterSaving, pointTransaction.PointTransactionId);
    // }
    //
    // [Fact]
    // public void RetrievePointTransactionByKey()
    // {
    //     const string constantName = $"{nameof(PointTransactionTests)}.{nameof(RetrievePointTransactionByKey)}";
    //     int          key;
    //
    //     using(var    context      = HelperMethods.CreatePointsContext(constantName))
    //     {
    //         Assert.Empty(context.PointTransactions);
    //         const string payer            = "SomePayerName";
    //         var          pointTransaction = new PointTransaction { Payer = payer };
    //         context.Add(pointTransaction);
    //         context.SaveChanges();
    //         key = pointTransaction.PointTransactionId;
    //     }
    //
    //     using(var context2 = HelperMethods.CreatePointsContext(constantName))
    //     {
    //         Assert.NotEmpty(context2.PointTransactions);
    //         var retrievedPointTransaction = context2.PointTransactions.SingleOrDefault(x => x.PointTransactionId == key);
    //         Assert.NotNull(retrievedPointTransaction);
    //         Assert.Equal(key, retrievedPointTransaction.PointTransactionId);
    //     }
    // }
    //
    // [Fact]
    // public void NullPayer_DbUpdateException()
    // {
    //     using var     context = HelperMethods.CreatePointsContext();
    //     const string? payer   = null;
    //     var           pointTransaction = new PointTransaction
    //                                      {
    //                                          Payer = payer!
    //                                      };
    //
    //     context.Add(pointTransaction);
    //     // ReSharper disable once AccessToDisposedClosure
    //     // ReSharper disable once ConvertToLocalFunction
    //     Action action = () => context.SaveChanges();
    //     Assert.Throws<DbUpdateException>(action);
    // }
    //
    // public static TheoryData<string, int, DateTime> SamplePointTransactions()
    // {
    //     var result = new TheoryData<string, int, DateTime>
    //                  {
    //                      // ReSharper disable StringLiteralTypo
    //                      { "DANNON", 1000, DateTime.Parse("2020-11-02T14:00:00Z") },
    //                      { "UNILEVER", 200, DateTime.Parse("2020-10-31T11:00:00Z") },
    //                      { "DANNON", -200, DateTime.Parse("2020-10-31T15:00:00Z") },
    //                      { "MILLER COORS", 10000, DateTime.Parse("2020-11-01T14:00:00Z") },
    //                      { "DANNON", 300, DateTime.Parse("2020-10-31T10:00:00Z") }
    //                      // ReSharper restore StringLiteralTypo
    //                  };
    //
    //     return result;
    // }
    //
    // [Theory]
    // [MemberData(nameof(SamplePointTransactions))]
    // public void ValidateSavedValues(string payer, int points, DateTime timeStamp)
    // {
    //     const string    constantName = $"{nameof(PointTransactionTests)}.{nameof(ValidateSavedValues)}";
    //     using var context = HelperMethods.CreatePointsContext(constantName);
    //     var pointTransaction = new PointTransaction
    //                            {
    //                                Payer = payer,
    //                                Points = points,
    //                                TimeStamp = timeStamp
    //                            };
    //     context.Add(pointTransaction);
    //     context.SaveChanges();
    //     var key = pointTransaction.PointTransactionId;
    //
    //     using var context2 = HelperMethods.CreatePointsContext(constantName);
    //     Assert.NotEmpty(context2.PointTransactions);
    //     var retrievedPointTransaction = context2.PointTransactions.SingleOrDefault(x => x.PointTransactionId == key);
    //     Assert.NotNull(retrievedPointTransaction);
    //     Assert.Equal(key,   retrievedPointTransaction.PointTransactionId);
    //     Assert.Equal(payer, retrievedPointTransaction.Payer);
    //     Assert.Equal(points, retrievedPointTransaction.Points);
    //     Assert.Equal(timeStamp, retrievedPointTransaction.TimeStamp);
    // }
    //
    // [Fact]
    // public void SaveNewPointTransaction_Assign_Key()
    // {
    //     using var context = HelperMethods.CreatePointsContext();
    //     Assert.Empty(context.PointTransactions);
    //     const string payer            = "SomePayer";
    //     const int    keyValue         = 10;
    //     var          pointTransaction = new PointTransaction { Payer = payer, PointTransactionId = keyValue };
    //     context.Add(pointTransaction);
    //     context.SaveChanges();
    //     Assert.NotEmpty(context.PointTransactions);
    //     Assert.Equal(keyValue, pointTransaction.PointTransactionId);
    // }
    //
    // [Fact]
    // public void SaveNewPointTransaction_Assign_same_key_to_multiple_records_InvalidOperationException()
    // {
    //     using var context = HelperMethods.CreatePointsContext();
    //     Assert.Empty(context.PointTransactions);
    //     const string payer            = "SomePayer";
    //     const int    keyValue         = 10;
    //     var          pointTransaction = new PointTransaction { Payer = payer, PointTransactionId = keyValue };
    //     context.Add(pointTransaction);
    //     context.SaveChanges();
    //
    //     var secondPointTransaction = new PointTransaction { Payer = payer, PointTransactionId = keyValue };
    //     
    //     // ReSharper disable once ConvertToLocalFunction
    //     // ReSharper disable once AccessToDisposedClosure
    //     Action action = () => context.Add(secondPointTransaction);
    //     Assert.Throws<InvalidOperationException>(action);
    // }
    //
    // [Fact]
    // public void New_PointTransaction_related_records_collections_are_not_null_and_are_empty()
    // {
    //     const string payer            = "SomePayer";
    //     var          pointTransaction = new PointTransaction { Payer = payer };
    //     const string constantName     = $"{nameof(PointTransactionTests)}.{nameof(New_PointTransaction_related_records_collections_are_not_null_and_are_empty)}";
    //     using(var context = HelperMethods.CreatePointsContext(constantName))
    //     {
    //         Assert.Empty(context.PointTransactions);
    //         Assert.Null(pointTransaction.AllocatedPoints);
    //         Assert.Null(pointTransaction.AvailablePoints);
    //         context.Add(pointTransaction);
    //         context.SaveChanges();
    //     }
    //
    //     using (var newContext = HelperMethods.CreatePointsContext(constantName))
    //     {
    //         var retrievedPointTransaction = newContext.PointTransactions.Find(pointTransaction.PointTransactionId);
    //         Assert.NotNull(retrievedPointTransaction);
    //
    //         Assert.Null(pointTransaction.AllocatedPoints);
    //         Assert.Null(pointTransaction.AvailablePoints);
    //
    //         Assert.NotNull(retrievedPointTransaction.AllocatedPoints);
    //         Assert.Null(retrievedPointTransaction.AvailablePoints);
    //         Assert.Empty(retrievedPointTransaction.AllocatedPoints);
    //     }
    //
    //
    // }
}