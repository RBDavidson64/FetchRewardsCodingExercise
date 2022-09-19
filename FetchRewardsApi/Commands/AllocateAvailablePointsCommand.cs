using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Allocates some or all spent points to an <see cref="AvailablePoints"/> entity record with unallocated points, creating
/// a new <see cref="AllocatedPoints"/> entity record and adjusting the payer's balance.
/// </summary>
/// <remarks>
/// No new or updated entity records are saved by <see cref="AllocateAvailablePointsCommand"/>. It is intended to be used
/// as part of a larger process whose root command handles saving changes. 
/// </remarks>
internal sealed class AllocateAvailablePointsCommand : IAllocateAvailablePoints
{
    private readonly IAllocatedPointsRepository _allocatedPointsRepository;
    private readonly IAvailablePointsRepository _availablePointsRepository;
    private readonly IPayerBalanceRepository         _payerBalanceRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="allocatedPointsRepository">A repository for adding a new <see cref="AllocatedPoints"/> entity record </param>
    /// <param name="availablePointsRepository">A repository for updating a <see cref="AvailablePoints"/> entity record.</param>
    /// <param name="payerBalanceRepository">A repository for updating a <see cref="PayerBalance"/> entity record.</param>
    public AllocateAvailablePointsCommand(IAllocatedPointsRepository allocatedPointsRepository, IAvailablePointsRepository availablePointsRepository, IPayerBalanceRepository payerBalanceRepository)
    {
        _allocatedPointsRepository   = allocatedPointsRepository;
        _availablePointsRepository   = availablePointsRepository;
        _payerBalanceRepository = payerBalanceRepository;
    }

    /// <inheritdoc />
    public (int PointsAllocated, bool PayerBalanceIsNegative) AllocateAvailablePoints(AvailablePoints availablePoints, int remainingPoints, SpentPoints spentPoints, CancellationToken cancellationToken)
    {
        var pointsToAllocate = availablePoints.UnallocatedPoints <= remainingPoints
                                   ? availablePoints.UnallocatedPoints
                                   : remainingPoints;

        var payerBalance = availablePoints.PayerBalance;
        payerBalance.Balance -= pointsToAllocate;
        _payerBalanceRepository.Update(payerBalance);

        var allocatedPoints = new AllocatedPoints
                              {
                                  AvailablePoints  = availablePoints,
                                  Payer            = availablePoints.Payer,
                                  PayerBalance     = availablePoints.PayerBalance,
                                  PointsAllocated  = pointsToAllocate,
                                  PointTransaction = availablePoints.PointTransaction,
                                  SpentPoints      = spentPoints,
                                  TimeStamp        = DateTime.Now
                              };
        availablePoints.AllocatedPoints    += pointsToAllocate;
        availablePoints.UnallocatedPoints  -= pointsToAllocate;
        availablePoints.AllPointsAllocated =  availablePoints.UnallocatedPoints == 0;
        _allocatedPointsRepository.Add(allocatedPoints);
        _availablePointsRepository.Update(availablePoints);
        return (pointsToAllocate, payerBalance.Balance < 0);
    }
}