using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FetchRewardsApi.ApiRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FetchRewardsApi.EntityModel;


/// <summary>
/// A ledger of the points added to payer balances.
/// </summary>
/// <remarks>
/// <see cref="PointTransaction"/> entity records are only added to the context, never updated or deleted.
///
/// When a <see cref="PointTransaction"/> entity record is created, a new <see cref="AvailablePoints"/> entity record
/// is created which contains all the information from the new <see cref="PointTransaction"/> entity record
/// as well as properties for tracking the points which have been allocated and how many remain.
///
/// The new <see cref="AvailablePoints"/> record should be used for all processing, leaving <see cref="PointTransaction"/>
/// entity records as an unadulterated accounting of the points added before any processing was done. 
/// </remarks>
internal sealed record PointTransaction
{
    
    private ICollection<AllocatedPoints> _allocatedPoints = null!;
    private PayerBalance                 _payerBalance    = null!;
    private AvailablePoints              _availablePoints = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public PointTransaction(){}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lazyLoader">Passed by the <see cref="DbContext"/> to enable lazy-loading of navigation properties.</param>
    public PointTransaction(ILazyLoader lazyLoader) => LazyLoader = lazyLoader;
    
    /// <summary>
    /// Used to lazy-load navigation properties.
    /// </summary>
    private ILazyLoader LazyLoader { get; } = null!;

    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PointTransactionId { get; init; }
    
    /// <summary>
    /// Copy of <see cref="EntityModel.PayerBalance.Payer"/>, kept here primarily to aid in debugging.  
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Payer { get; init; } = null!;

    /// <summary>
    /// The points added to the payer's balance.
    /// </summary>
    /// <value>
    /// The points added can be a negative number, so long as this would not result in a negative <see cref="EntityModel.PayerBalance.Balance"/>
    /// </value>
    [Required]
    public int Points { get; init; }
    
    /// <summary>
    /// The <see cref="DateTime"/> specified by the <see cref="AddPointsTransaction"/> used to create this record.
    /// </summary>
    [Required]
    public DateTime TimeStamp { get; init; }

    /// <summary>
    /// Navigation property to the <see cref="PayerBalance"/> entity record
    /// </summary>
    [Required]
    public PayerBalance PayerBalance
    {
        get => LazyLoader.Load(this, ref _payerBalance!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _payerBalance = value;
    }

    /// <summary>
    /// Navigation property to the <see cref="AvailablePoints"/> entity record created at the same time as this record..
    /// </summary>
    public AvailablePoints AvailablePoints
    {
        get => LazyLoader.Load(this, ref _availablePoints!)!; 
        set => _availablePoints = value;
    }

    /// <summary>
    /// Navigation property to all <see cref="AllocatedPoints"/> entity records which track which points added by this record have been allocated. 
    /// </summary>
    [InverseProperty(nameof(EntityModel.AllocatedPoints.PointTransaction))]
    public ICollection<AllocatedPoints> AllocatedPoints 
    {
        get => LazyLoader.Load(this, ref _allocatedPoints!)!; 
        set => _allocatedPoints = value;
    }
    
}