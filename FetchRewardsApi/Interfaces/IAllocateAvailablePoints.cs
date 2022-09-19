using FetchRewardsApi.EntityModel;

namespace FetchRewardsApi.Interfaces;

internal interface IAllocateAvailablePoints
{
    /// <summary>
    /// Allocates points to <paramref name="availablePoints"/>, updating <paramref name="availablePoints.PayBalance"/>, and creating
    /// a new <see cref="AllocatedPoints"/> entity record to record the transaction.
    /// </summary>
    /// <param name="availablePoints">The <see cref="AvailablePoints"/> entity record to be updated.</param>
    /// <param name="remainingPoints">The points which can be allocated.</param>
    /// <param name="spentPoints">The originating <see cref="SpentPoints"/> record, which will be assigned as a foreign parent
    /// of the new <see cref="AllocatedPoints"/> entity record.</param>
    /// <param name="cancellationToken">A means of cancelling the process.</param>
    /// <returns>
    /// A <see cref="Tuple"/> containing the points allocated, and if the operation resulted in a negative <see cref="PayerBalance.Balance"/>
    /// </returns>
    /// <remarks>
    /// Allocating points can make multiple adjustments to a payer's balance, and each <see cref="AvailablePoints"/> entity record
    /// might have either a positive or negative <see cref="AvailablePoints.UnallocatedPoints"/>.
    ///
    /// This can create the case where
    /// processing one <see cref="AvailablePoints"/> entity record might result in a negative <see cref="PayerBalance.Balance"/>,
    /// but processing of a later <see cref="AvailablePoints"/> entity record results in the same <see cref="PayerBalance.Balance"/>
    /// becoming positive again.
    ///
    /// For this reason, no error is reported by <see cref="AllocateAvailablePoints"/>. Instead the returned <see cref="Tuple"/>
    /// includes a <see cref="Boolean"/> which warns the calling command that a negative <see cref="PayerBalance.Balance"/> occurred.
    ///
    /// The calling command, after processing all effected <see cref="AvailablePoints"/> entity records, is responsible for seeing
    /// if any <see cref="PayerBalance"/> entity records have a negative <see cref="PayerBalance.Balance"/>. 
    /// </remarks>
    (int PointsAllocated, bool PayerBalanceIsNegative) AllocateAvailablePoints(AvailablePoints availablePoints, int remainingPoints, SpentPoints spentPoints, CancellationToken cancellationToken);
}