using FetchRewardsApi.ApiRecords;

namespace FetchRewardsApi.Interfaces;

public interface IGetBalances
{
    /// <summary>
    /// Gets a collection of all payers and their balances.
    /// </summary>
    /// <param name="cancellationToken">The token which can be used to cancel the operation.</param>
    /// <returns>A <see cref="Results.Ok"/> containing a collection of <see cref="Balance"/> records.</returns>
    Task<IResult> GetBalances(CancellationToken cancellationToken);
}