using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.Interfaces;

internal interface ICreatePointTransaction
{
    /// <summary>
    /// Creates a new <see cref="PointTransaction"/> entity record.
    /// </summary>
    /// <param name="payerBalance">The <see cref="PayerBalance"/> entity record whose <see cref="PayerBalance.Balance"/> will be effected by the creation of the new <see cref="PointTransaction"/> entity record.</param>
    /// <param name="points">The points to assign to <see cref="PointTransaction.Points"/></param>
    /// <param name="timeStamp">The <see cref="DateTime"/> assigned to <see cref="PointTransaction.TimeStamp"/></param>
    /// <returns>The new <see cref="PointTransaction"/> record.</returns>
    PointTransaction CreatePointTransaction(PayerBalance payerBalance, int points, DateTime timeStamp);

}