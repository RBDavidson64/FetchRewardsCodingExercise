using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.EntityModel;

internal class PointsContext : DbContext
{
    public PointsContext(DbContextOptions options) : base(options)
    {}

    public DbSet<PointTransaction> PointTransactions { get; set; }
}