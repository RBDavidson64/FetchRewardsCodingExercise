using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;

namespace FetchRewardsApi.Commands;

/// <summary>
/// Gets a collection of all payers and their balances.
/// </summary>
internal sealed class GetBalancesCommand
    : IGetBalances
{
    
    private readonly IPayerBalanceRepository _payerBalanceRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="payerBalanceRepository">The repository used to read <see cref="PayerBalance"/> entity records</param>
    public GetBalancesCommand(IPayerBalanceRepository payerBalanceRepository) => _payerBalanceRepository = payerBalanceRepository;

    /// <inheritdoc />
    public async Task<IResult> GetBalances(CancellationToken cancellationToken)
    {
        var   results    = await _payerBalanceRepository.GetBalances(cancellationToken).ConfigureAwait(false);
        return Results.Ok(results);
    }
}