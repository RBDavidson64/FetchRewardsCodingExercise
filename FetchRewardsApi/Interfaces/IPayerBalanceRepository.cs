using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Interfaces;

internal interface IPayerBalanceRepository 
{
    /// <summary>
    /// Get an <see cref="PayerBalance"/> entity record by the name of the payer.
    /// </summary>
    /// <param name="payerName">The name of the payer</param>
    /// <param name="cancellationToken">A means of cancelling the operation.</param>
    /// <returns>
    /// The <see cref="PayerBalance"/> entity record whose <see cref="PayerBalance.Payer"/> property equals <paramref name="payerName"/>,
    /// or null if no such record exists.
    /// </returns>
    public Task<PayerBalance?> GetByPayer(string payerName, CancellationToken cancellationToken);
    
    /// <summary>
    /// Marks an existing <see cref="PayerBalance"/> entity record as <see cref="EntityState.Modified"/>
    /// </summary>
    /// <param name="payerBalance">The entity record to mark as <see cref="EntityState.Modified"/></param>
    void                       Update(PayerBalance                       payerBalance);
    
    /// <summary>
    /// Asynchronously sum the <see cref="PayerBalance.Balance"/> property across all payers.
    /// </summary>
    /// <param name="cancellationToken">A means of cancelling the operation.</param>
    /// <returns>The sum of the <see cref="PayerBalance.Balance"/> property across all payers.</returns>
    Task<int>                  GetTotalBalance(CancellationToken cancellationToken);
    
    /// <summary>
    /// Add a new <see cref="PayerBalance"/> entity record to the context.
    /// </summary>
    /// <param name="payerBalance">The entity record to add to the context.</param>
    void                            Add(PayerBalance                                payerBalance);
    
    /// <summary>
    /// Asynchronously gets any <see cref="PayerBalance"/> entity record with a negative <see cref="PayerBalance.Balance"/>.
    /// </summary>
    /// <param name="cancellationToken">A means of cancelling the operation.</param>
    /// <returns>An <see cref="ICollection{PayBalance}"/></returns>
    Task<ICollection<PayerBalance>> GetPayersWithNegativeBalances(CancellationToken cancellationToken);
    
    /// <summary>
    /// Get a <see cref="ICollection{Balance}"/> api records reflecting the balances for all payers. 
    /// </summary>
    /// <param name="cancellationToken">A means of cancelling the operation</param>
    /// <returns>An <see cref="ICollection{Balance}"/></returns>
    Task<ICollection<Balance>>      GetBalances(CancellationToken cancellationToken);
}