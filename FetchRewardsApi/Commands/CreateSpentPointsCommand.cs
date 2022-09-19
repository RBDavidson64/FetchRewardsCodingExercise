using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Creates a new <see cref="SpentPoints"/> entity record.
/// </summary>
/// <remarks>
/// The new record is added to the context, but is not saved.  This command is intended to be used as part of
/// a larger process which is responsible for saving any changes.
/// </remarks>
internal sealed class CreateSpentPointsCommand : ICreateSpentPoints
{
    private readonly IPayerBalanceRepository _payerBalanceRepository;
    private readonly ISpentPointsRepository  _spentPointsRepository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="payerBalanceRepository">A repository used to verify that the total unallocated points is greater than or equal to the points being spent. </param>
    /// <param name="spentPointsRepository">A repository used to add the new record to the context.</param>
    public CreateSpentPointsCommand(IPayerBalanceRepository payerBalanceRepository, ISpentPointsRepository spentPointsRepository)
    {
        _payerBalanceRepository     = payerBalanceRepository;
        _spentPointsRepository = spentPointsRepository;
    }

    /// <inheritdoc />
    public async Task<(SpentPoints? SpentPoints, IResult? Problem)> CreateSpentPoints(int pointsToSpend, CancellationToken cancellationToken)
    {
        if (pointsToSpend < 0)
        {
            var titleOperationWouldCorruptData = string.Format(ErrorMessages.TitleOperationWouldCorruptData, $"{nameof(CreateSpentPointsCommand)}.{nameof(CreateSpentPoints)}");
            return (null,  Results.Problem(detail: ErrorMessages.NegativeSpendPointsError, statusCode: StatusCodes.Status422UnprocessableEntity, title: titleOperationWouldCorruptData )); 
        }
        
        var totalAvailablePoints = await _payerBalanceRepository.GetTotalBalance(cancellationToken).ConfigureAwait(false);

        if (pointsToSpend > totalAvailablePoints)
        {
            var titleOperationWouldCorruptData = string.Format(ErrorMessages.TitleOperationWouldCorruptData, $"{nameof(CreateSpentPointsCommand)}.{nameof(CreateSpentPoints)}");
            return (null,  Results.Problem(detail: ErrorMessages.SpendingMorePointsThanAvailableError, statusCode: StatusCodes.Status422UnprocessableEntity, title: titleOperationWouldCorruptData )); 
        }
        
        var spentPoints = new SpentPoints
                          {
                              PointsSpent = pointsToSpend,
                              TimeStamp   = DateTime.Now
                          };
        _spentPointsRepository.Add(spentPoints);
        return (spentPoints, null);
    }
}