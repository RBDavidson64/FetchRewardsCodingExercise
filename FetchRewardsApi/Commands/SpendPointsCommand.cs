using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Allocates spent points
/// </summary>
/// <remarks>
/// This command does save new and updated entity records.
/// </remarks>
internal sealed class SpendPointsCommand
    : ISpendPoints
{
    private readonly PointsContext      _context;
    private readonly ICreateSpentPoints _createSpentPoints;
    private readonly IAllocatePoints    _allocatePoints;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The context used to save new and updated entity records. </param>
    /// <param name="createSpentPoints">A command used to validate the spent points, and create a new <see cref="SpentPoints"/> entity record.</param>
    /// <param name="allocatePoints">
    /// A command used to allocate points to <see cref="AvailablePoints"/> entity records,
    /// create new <see cref="AllocatedPoints"/> entity records, and update <see cref="PayerBalance"/> entity records.
    /// </param>
    public SpendPointsCommand(
        PointsContext      context,
        ICreateSpentPoints createSpentPoints,
        IAllocatePoints    allocatePoints
    )
    {
        _context           = context;
        _createSpentPoints = createSpentPoints;
        _allocatePoints    = allocatePoints;
    }

    /// <inheritdoc />
    public async Task<IResult> SpendPoints(SpendPointsTransaction spendPointsTransaction, CancellationToken cancellationToken)
    {
        var (spentPoints, problem) = await _createSpentPoints.CreateSpentPoints(spendPointsTransaction.Points, cancellationToken).ConfigureAwait(false);
        if (problem is not null) return problem;
        var (success, result) = await _allocatePoints.AllocatePoints(spentPoints!, cancellationToken);
        if (success) await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }
}