using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;

namespace FetchRewardsApi.Interfaces;

public interface ISpendPointsResults
{
    /// <summary>
    /// Records the net changes made to payer balances as part of processing the <see cref="SpendPointsCommand"/>. 
    /// </summary>
    /// <param name="payer"> The payer where points shall be subtracted. </param>
    /// <param name="points">
    /// The points to subtract from the payer.
    /// <value>
    /// Negative <paramref name="points"/> are allowed, which will effectively add points to the payer.
    /// </value>
    /// </param>
    void Subtract(string payer, int points);
    
    /// <summary>
    /// All the <see cref="SpendPointsResult"/> records created by calls to <see cref="Subtract"/>
    /// </summary>
    SpendPointsResult[] Results { get; }
}