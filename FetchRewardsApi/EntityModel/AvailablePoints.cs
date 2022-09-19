using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FetchRewardsApi.EntityModel;

/// <summary>
/// Records the points originally added to a payer's balance, the points which have been allocated, and the remaining points which can be allocated..
/// </summary>
/// <remarks>
/// When a new <see cref="PointTransaction"/> record is added, a corresponding <see cref="AvailablePoints"/>
/// record is also added, with <see cref="OriginalUnallocatedPoints"/> and <see cref="UnallocatedPoints"/> assigned the value of
/// <see cref="EntityModel.PointTransaction.Points"/>.
///
/// When a <see cref="SpentPoints"/> transaction is added, records from <see cref="AvailablePoints"/>
/// are processed to:
///     * create new <see cref="EntityModel.AllocatedPoints"/> records
///     * add points to property <see cref="AllocatedPoints"/>
///     * subtract points from property <see cref="UnallocatedPoints"/>
///     * assign true to property <see cref="AllPointsAllocated"/> once property <see cref="UnallocatedPoints"/> equals 0. 
/// </remarks>
/// <remarks>
/// The payer's balances should always equal the sum of the <see cref="AvailablePoints.UnallocatedPoints"/> for that <see cref="Payer"/>
/// </remarks>
[Index(nameof(AllPointsAllocated), nameof(PointTransactionTimeStamp))]
internal sealed record AvailablePoints
{
    private          PointTransaction _pointTransaction = null!;
    private  PayerBalance     _payerBalance             = null!;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public AvailablePoints(){}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lazyLoader">Passed by the <see cref="DbContext"/> to enable lazy-loading of navigation properties.</param>
    public AvailablePoints(ILazyLoader lazyLoader) => LazyLoader = lazyLoader;
    
    /// <summary>
    /// Used to lazy-load navigation properties.
    /// </summary>
    private ILazyLoader LazyLoader { get; } = null!;
    
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AvailablePointsId { get; init; }
    
    /// <summary>
    /// The payer the <see cref="UnallocatedPoints"/> are associated with. 
    /// </summary>
    /// <remarks>
    /// Value is copied from the <see cref="PointTransaction"/> where the <see cref="UnallocatedPoints"/> originated.
    /// If is also a copy of the <see cref="EntityModel.PayerBalance.Payer"/> field.
    /// It is maintained here to make operations quicker and problems easier to debug.
    /// </remarks>
    [Required(AllowEmptyStrings = false)]
    public string Payer { get; init; } = null!;
    
    /// <summary>
    /// The value of the <see cref="EntityModel.PointTransaction.Points"/> used to create this record
    /// </summary>
    /// <remarks>
    /// This value is maintained primarily for debugging purposes. It should only be assigned when the record is created.  
    /// </remarks>
    [Required]
    public int OriginalUnallocatedPoints { get; init; }
    
    /// <summary>
    /// The total number of points which have been allocated by <see cref="SpentPoints"/> transactions.
    /// </summary>
    [Required]
    public int AllocatedPoints { get; set; }
    
    /// <summary>
    /// Points which have not been allocated by a <see cref="SpentPoints"/> transaction. 
    /// </summary>
    public int UnallocatedPoints { get;  set; }
    
    /// <summary>
    /// Assigned true when <see cref="UnallocatedPoints"/> equals 0, otherwise it should be false.
    /// </summary>
    public bool AllPointsAllocated { get;  set; }
    
    /// <summary>
    /// The <see cref="EntityModel.PointTransaction.TimeStamp"/> from the <see cref="PointTransaction"/> record where the <see cref="UnallocatedPoints"/> originated.
    /// </summary>
    /// <remarks>
    /// This field is used so that when points are spent, they are allocated using the oldest transactions first.
    /// It is copied from the <see cref="PointTransaction"/> record where the <see cref="UnallocatedPoints"/> originated.
    /// It is maintained here to improve performance and aid in debugging/troubleshooting.
    /// </remarks>
    [Required]
    public DateTime PointTransactionTimeStamp { get; init; }    
    
    /// <summary>
    /// The <see cref="EntityModel.PointTransaction.PointTransactionId"/> 
    /// </summary>
    /// <remarks>
    /// This is required since <see cref="AvailablePoints"/> and <see cref="PointTransaction"/> have a 1-1 relationship
    /// which made it impossible for Entity Framework to determine which was the parent/child record.
    /// </remarks>
    [ForeignKey(nameof(PointTransaction))]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public int PointTransactionId { get; set; }

    /// <summary>
    /// Navigation property to the <see cref="PointTransaction"/> where the <see cref="UnallocatedPoints"/> originated.
    /// </summary>
    [Required]
    [InverseProperty(nameof(EntityModel.PointTransaction.AvailablePoints))]
    public PointTransaction PointTransaction
    {
        get => LazyLoader.Load(this, ref _pointTransaction!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _pointTransaction = value;
    }

    /// <summary>
    /// Navigation record to the The <see cref="PayerBalance"/> record.
    /// </summary>
    [Required]
    public PayerBalance PayerBalance
    {
        get => LazyLoader.Load(this, ref _payerBalance!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _payerBalance = value;
    }
}