using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FetchRewardsApi.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FetchRewardsApi.EntityModel;

/// <summary>
/// A record of the points allocated by the <see cref="SpendPointsCommand"/>.
/// </summary>
internal sealed record AllocatedPoints
{
    private PointTransaction _pointTransaction = null!;
    private AvailablePoints  _availablePoints  = null!;
    private SpentPoints      _spentPoints      = null!;
    private PayerBalance     _payerBalance             = null!;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public AllocatedPoints(){}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lazyLoader">Passed by the <see cref="DbContext"/> to enable lazy-loading of navigation properties.</param>
    public AllocatedPoints(ILazyLoader lazyLoader) => LazyLoader = lazyLoader;
    
    /// <summary>
    /// Used to lazy-load navigation properties.
    /// </summary>
    private ILazyLoader LazyLoader { get; } = null!;
    
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AllocatedPointsId { get; init; }
    
    /// <summary>
    /// Copy of <see cref="EntityModel.PayerBalance.Payer"/>, kept here primarily to aid in performance and debugging.  
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Payer { get; init; } = null!;

    /// <summary>
    /// The points which have been allocated
    /// </summary>
    [Required]
    public int PointsAllocated { get; init; }
    
    /// <summary>
    /// The <see cref="DateTime"/> when this record was created.
    /// </summary>
    [Required]
    public  DateTime TimeStamp { get; init; }

    /// <summary>
    /// Navigation property to the <see cref="PointTransaction"/> where the point originated.
    /// </summary>
    [Required]
    public PointTransaction PointTransaction
    {
        get => LazyLoader.Load(this, ref _pointTransaction!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _pointTransaction = value;
    }

    /// <summary>
    /// Navigation property to the <see cref="EntityModel.AvailablePoints"/> record used to create this record. 
    /// </summary>
    [Required]
    public AvailablePoints AvailablePoints
    {
        get => LazyLoader.Load(this, ref _availablePoints!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _availablePoints = value;
    }

    /// <summary>
    /// Navigation property to the <see cref="EntityModel.SpentPoints"/> transaction which triggered the creation of this record
    /// and which the <see cref="PointsAllocated"/> apply.
    /// </summary>
    [Required]
    public SpentPoints SpentPoints
    {
        get => LazyLoader.Load(this, ref _spentPoints!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _spentPoints = value;
    }

    /// <summary>
    /// Navigation property to the <see cref="Payer"/> to whom the <see cref="PointsAllocated"/> were applied.
    /// </summary>
    [Required]
    public PayerBalance PayerBalance
    {
        get => LazyLoader.Load(this, ref _payerBalance!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _payerBalance = value;
    }
}