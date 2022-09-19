using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Processes <see cref="AllocatePoints"/> entity records, allocating <see cref="SpentPoints"/>
/// </summary>
internal sealed class AllocatePointsCommand : IAllocatePoints
{
    private readonly IAvailablePointsRepository _availablePointsRepository;
    private readonly IAllocateAvailablePoints   _allocateAvailablePoints;
    private readonly ISpendPointsResults        _spendPointsResults;
    private readonly IPayerBalanceRepository    _payerBalanceRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="availablePointsRepository">A repository for reading the <see cref="AvailablePoints"/> entity records which will be processed. </param>
    /// <param name="allocateAvailablePoints">The <see cref="AllocatedPoints"/> entity records created by the processing of <see cref="AvailablePoints"/> entity records.</param>
    /// <param name="spendPointsResults">Used to store the changes made to payer balances.</param>
    /// <param name="payerBalanceRepository">A repository used after all points have been allocated to verify that no <see cref="PayerBalance.Balance"/> has a negative value.</param>
    public AllocatePointsCommand(IAvailablePointsRepository availablePointsRepository, IAllocateAvailablePoints allocateAvailablePoints, ISpendPointsResults spendPointsResults, IPayerBalanceRepository payerBalanceRepository)
    {
        _availablePointsRepository   = availablePointsRepository;
        _allocateAvailablePoints     = allocateAvailablePoints;
        _spendPointsResults          = spendPointsResults;
        _payerBalanceRepository = payerBalanceRepository;
    }

    /// <inheritdoc />
    public async Task<(bool Success, IResult Result)> AllocatePoints(SpentPoints spentPoints, CancellationToken cancellationToken)
    {
        var pointsToAllocate      = spentPoints.PointsSpent;
        var availablePointsEnumerable = _availablePointsRepository.UnallocatedPointsOrderedByTimeStamp(pointsToAllocate);
        var checkPayerBalances   = false;
        foreach (var availablePoints in availablePointsEnumerable)
        {
            var (pointsAllocated, payerBalanceIsNegative) = _allocateAvailablePoints.AllocateAvailablePoints(availablePoints, pointsToAllocate, spentPoints, cancellationToken);
            checkPayerBalances                            = checkPayerBalances || payerBalanceIsNegative;
            _spendPointsResults.Subtract(availablePoints.Payer, pointsAllocated);
            pointsToAllocate -= pointsAllocated;
            if (pointsToAllocate == 0) break;
        }

        if (!checkPayerBalances) return (true, Results.Ok(_spendPointsResults.Results));
        
        var payersWithNegativeBalances = await _payerBalanceRepository.GetPayersWithNegativeBalances(cancellationToken).ConfigureAwait(false);
        if (!payersWithNegativeBalances.Any()) return (true, Results.Ok(_spendPointsResults.Results));
        
        var titleOperationWouldCorruptData = string.Format(ErrorMessages.TitleOperationWouldCorruptData, $"{nameof(AllocatePointsCommand)}.{nameof(AllocatePoints)}");
        return (false, Results.Problem(type: StatusCodes.Status422UnprocessableEntity.ToString(), title: titleOperationWouldCorruptData, detail: ErrorMessages.NegativeBalanceError, statusCode: StatusCodes.Status422UnprocessableEntity));
    }
}