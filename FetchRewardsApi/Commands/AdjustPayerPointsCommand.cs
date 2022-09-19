using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Adjusts an existing <see cref="PayerBalance.Balance"/>, or adds a new <see cref="PayerBalance"/> entity record.
/// </summary>
internal sealed class AdjustPayerPointsCommand : IAdjustPayerPoints
{
    private readonly IPayerBalanceRepository _payerBalancesRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="payerBalancesRepository">The repository used to retrieve, add, and update records.</param>
    public AdjustPayerPointsCommand(IPayerBalanceRepository payerBalancesRepository) => _payerBalancesRepository = payerBalancesRepository;

    /// <inheritdoc />
    public async Task<(PayerBalance? PayerBalance, IResult? Problem)> AdjustPayerPoints(string payer, int pointToAdd, CancellationToken cancellationToken)
    {

        var payerBalance            = await _payerBalancesRepository.GetByPayer(payer, cancellationToken).ConfigureAwait(false);

        var newPayerBalance = (payerBalance?.Balance ?? 0) + pointToAdd;

        if (newPayerBalance < 0)
        {
            var titleOperationWouldCorruptData = string.Format(ErrorMessages.TitleOperationWouldCorruptData, $"{nameof(AdjustPayerPointsCommand)}.{nameof(AdjustPayerPoints)}");
            return (null, Results.Problem(type: StatusCodes.Status422UnprocessableEntity.ToString(), title: titleOperationWouldCorruptData, detail: ErrorMessages.NegativeBalanceError, statusCode: StatusCodes.Status422UnprocessableEntity));
        }

        if (payerBalance is null)
        {
            payerBalance = new PayerBalance { Payer = payer, Balance = newPayerBalance };
            _payerBalancesRepository.Add(payerBalance);
            return (payerBalance, null); 
        }

        payerBalance.Balance = newPayerBalance;
        _payerBalancesRepository.Update(payerBalance);

        return (payerBalance, null);
    }
}