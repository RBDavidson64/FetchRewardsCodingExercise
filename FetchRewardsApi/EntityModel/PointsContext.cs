using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.EntityModel;

internal sealed class PointsContext : DbContext
{
    public PointsContext(DbContextOptions options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }

    public DbSet<PointTransaction>        PointTransactions          { get; set; } = null!;
    public DbSet<AllocatedPoints>         AllocatedPointsSet         { get; set; } = null!;

    public DbSet<AvailablePoints>         AvailablePointsSet         { get; set; } = null!;
    public DbSet<SpentPoints>             SpentPointsSet             { get; set; } = null!;
    public DbSet<PayerBalance>            PayerBalances              { get; set; } = null!;
}