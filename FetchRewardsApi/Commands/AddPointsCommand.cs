using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Adds points to the appropriate <see cref="PointsContext.PayerBalances"/> entity record, and adds a new <see cref="PointTransaction"/>
/// and <see cref="AvailablePoints"/> entity records to represent the added points. 
/// </summary>
/// <remarks>
/// If a <see cref="PayerBalance"/> entity record does not exist for the specified payer, a new <see cref="PayerBalance"/> entity record will be created.
/// </remarks>
internal sealed class AddPointsCommand : IAddPoints
{
    private readonly PointsContext           _context;
    private readonly IAdjustPayerPoints      _adjustPayerPoints;
    private readonly ICreatePointTransaction _createPointTransaction;
    private readonly ICreateAvailablePoints  _createAvailablePoints;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="PointsContext"/> used to make changes and add records.</param>
    /// <param name="adjustPayerPoints">A command to adjust a payer's points, or add a new <see cref="PayerBalance"/> entity record if one does not exist.</param>
    /// <param name="createPointTransaction">A command to create a new <see cref="PointTransaction"/> entity record.</param>
    /// <param name="createAvailablePoints">A command to create a new <see cref="AvailablePoints"/> entity record.</param>
    /// <remarks>
    /// The <paramref name="adjustPayerPoints"/>, <paramref name="createPointTransaction"/>, and <paramref name="createAvailablePoints"/>
    /// commands all use the passed <paramref name="context"/> to read, add, and update records, but do not save any changes.
    /// All changes are saved by the <see cref="AddPoints"/> method after all commands have completed successfully.
    /// </remarks>
    public AddPointsCommand(PointsContext context, IAdjustPayerPoints adjustPayerPoints, ICreatePointTransaction createPointTransaction, ICreateAvailablePoints createAvailablePoints)
    {
        _context                = context;
        _adjustPayerPoints      = adjustPayerPoints;
        _createPointTransaction = createPointTransaction;
        _createAvailablePoints  = createAvailablePoints;
    }

    /// <inheritdoc />
    public async Task<IResult> AddPoints(AddPointsTransaction addPointsTransaction, CancellationToken cancellationToken)
    {
        var (payerBalance, problem) = await _adjustPayerPoints.AdjustPayerPoints(addPointsTransaction.Payer, addPointsTransaction.Points, cancellationToken).ConfigureAwait(false);
        if (problem is not null) return problem;
        var pointTransaction = _createPointTransaction.CreatePointTransaction(payerBalance!, addPointsTransaction.Points, addPointsTransaction.TimeStamp);
        _createAvailablePoints.CreateAvailablePoints(pointTransaction);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Results.Ok();
    }
}