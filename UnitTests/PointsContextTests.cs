using FetchRewardsApi.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace UnitTests;

public class PointsContextTests
{
    private static PointsContext CreatePointsContext()
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseInMemoryDatabase(nameof(PointsContext));
        return new PointsContext(builder.Options);
    }

    [Fact]
    public void Create()
    {
        using var context = CreatePointsContext();
        Assert.NotNull(context);
    }

    [Fact]
    public void GetPointTransactions()
    {
        using var context           = CreatePointsContext();
        var       pointTransactions = context.PointTransactions;
        Assert.NotNull(pointTransactions);
        Assert.Empty(pointTransactions);
    }

    [Fact]
    public void SaveNewPointTransaction()
    {
        using var context          = CreatePointsContext();
        var       pointTransaction = new PointTransaction();
        context.Add(pointTransaction);
        const int expected = 1;
        Assert.Equal(expected, context.SaveChanges());
    }
    
    [Fact]
    public void SaveNewPointTransaction_AutoIncrementKey()
    {
        using var context                   = CreatePointsContext();
        Assert.Empty(context.PointTransactions);
        var       pointTransaction          = new PointTransaction();
        const int expectedValueBeforeSaving = 0;
        Assert.Equal(expectedValueBeforeSaving, pointTransaction.PointTransactionId);
        context.Add(pointTransaction);
        context.SaveChanges();
        Assert.NotEmpty(context.PointTransactions);
        const int expectedValueAfterSaving = 1;
        Assert.Equal(expectedValueAfterSaving, pointTransaction.PointTransactionId);
    }

    [Fact]
    public void RetrievePointTransactionByKey()
    {
        using var    context          = CreatePointsContext();
        Assert.Empty(context.PointTransactions);
        var          pointTransaction = new PointTransaction();
        const string payer            = "SomePayerName";
        pointTransaction.Payer = payer;
        context.Add(pointTransaction);
        context.SaveChanges();
        var key = pointTransaction.PointTransactionId;

        using var context2 = CreatePointsContext();
        Assert.NotEmpty(context2.PointTransactions);
        var retrievedPointTransaction = context2.PointTransactions.SingleOrDefault(x => x.PointTransactionId == key);
        Assert.NotNull(retrievedPointTransaction);
        Assert.Equal(key,   retrievedPointTransaction.PointTransactionId);
        Assert.Equal(payer, retrievedPointTransaction.Payer);
    }
}