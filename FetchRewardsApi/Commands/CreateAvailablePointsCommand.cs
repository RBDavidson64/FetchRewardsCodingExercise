using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Creates a new <see cref="AvailablePoints"/> entity record
/// </summary>
/// <remarks>
/// The new record is added to the context, but is not saved.  This command is intended to be used as part of
/// a larger process which is responsible for saving any changes.
/// </remarks>
internal sealed class CreateAvailablePointsCommand : ICreateAvailablePoints
{
    private readonly IAvailablePointsRepository _availableBalanceRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="availableBalanceRepository">A repository used to add the new <see cref="AvailablePoints"/> entity record.</param>
    public CreateAvailablePointsCommand(IAvailablePointsRepository availableBalanceRepository) => _availableBalanceRepository = availableBalanceRepository;

    /// <inheritdoc />
    public AvailablePoints CreateAvailablePoints(PointTransaction pointTransaction)
    {
        var availablePoints = new AvailablePoints
                              {
                                  AllocatedPoints           = 0,
                                  AllPointsAllocated        = false,
                                  OriginalUnallocatedPoints = pointTransaction.Points,
                                  Payer                     = pointTransaction.Payer,
                                  PayerBalance              = pointTransaction.PayerBalance,
                                  PointTransaction          = pointTransaction,
                                  PointTransactionTimeStamp = pointTransaction.TimeStamp,
                                  UnallocatedPoints         = pointTransaction.Points
                              };

        pointTransaction.AvailablePoints = availablePoints;
        _availableBalanceRepository.Add(availablePoints);
        return availablePoints;
    }
}