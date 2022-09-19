using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Creates a new <see cref="PointTransaction"/> entity record.
/// </summary>
/// <remarks>
/// The new record is added to the context, but is not saved.  This command is intended to be used as part of
/// a larger process which is responsible for saving any changes.
/// </remarks>
internal sealed class CreatePointTransactionCommand : ICreatePointTransaction
{
    private readonly IPointTransactionRepository _pointTransactionRepository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pointTransactionRepository">The repository used to add the new record to the context.</param>
    public CreatePointTransactionCommand(IPointTransactionRepository pointTransactionRepository) => _pointTransactionRepository = pointTransactionRepository;

    /// <inheritdoc />
    public PointTransaction CreatePointTransaction(PayerBalance payerBalance, int points, DateTime timeStamp)
    {

        var pointTransaction = new PointTransaction
                               {
                                   Payer = payerBalance.Payer, PayerBalance = payerBalance, Points = points, TimeStamp = timeStamp
                               };

        _pointTransactionRepository.Add(pointTransaction);
        return pointTransaction;
    }
}