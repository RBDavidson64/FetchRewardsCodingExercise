using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Repositories;

/// <summary>
/// A repository used to read, add, and update <see cref="PayerBalance"/> entity records.
/// </summary>
internal sealed class PayerBalanceRepository : IPayerBalanceRepository
{
    private readonly GenericRepository<PayerBalance> _repository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> used to read, add, and update entity records.</param>
    public PayerBalanceRepository(PointsContext context) => _repository = new GenericRepository<PayerBalance>(context);

    /// <inheritdoc />
    public async Task<PayerBalance?> GetByPayer(string   payerName, CancellationToken cancellationToken) => await _repository.SingleOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(payerName, x.Payer), cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public       void                Update(PayerBalance payerBalance) => _repository.Update(payerBalance);

    /// <inheritdoc />
    public async Task<int>                       GetTotalBalance(CancellationToken       cancellationToken) => await _repository.Sum(x => x.Balance, cancellationToken).ConfigureAwait(false);
    
    /// <inheritdoc />
    public       void                            Add(PayerBalance                                payerBalance)      => _repository.Add(payerBalance);
    
    /// <inheritdoc />
    public async Task<ICollection<PayerBalance>> GetPayersWithNegativeBalances(CancellationToken cancellationToken) => await _repository.Get( cancellationToken, x => x.Balance < 0).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<ICollection<Balance>> GetBalances(CancellationToken cancellationToken)
    {
        var payerBalances = await _repository.Get(cancellationToken).ConfigureAwait(false); 
        return payerBalances.Select(x => new Balance(x.Payer, x.Balance)).ToArray();
    }
}