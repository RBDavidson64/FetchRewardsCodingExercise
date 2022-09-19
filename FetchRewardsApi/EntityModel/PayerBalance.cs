using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FetchRewardsApi.EntityModel;

/// <summary>
/// The payer and the balance of their unallocated points 
/// </summary>
[Index(nameof(Payer), IsUnique = true)]
internal sealed record PayerBalance
{
    private ICollection<PointTransaction>        _pointTransactions       = null!;
    private ICollection<AvailablePoints>         _availablePoints         = null!;
    private ICollection<AllocatedPoints>         _allocatedPoints         = null!;
   
    /// <summary>
    /// Constructor
    /// </summary>
    public PayerBalance(){}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lazyLoader">Passed by the <see cref="DbContext"/> to enable lazy-loading of navigation properties.</param>
    public PayerBalance(ILazyLoader lazyLoader) => LazyLoader = lazyLoader;
    
    /// <summary>
    /// Used to lazy-load navigation properties.
    /// </summary>
    private ILazyLoader LazyLoader { get; } = null!;
    
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PayerBalanceId { get; init; }
    
    /// <summary>
    /// The name of the payer
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Payer { get; init; } = null!;

    /// <summary>
    /// How many unallocated points the payer has.
    /// </summary>
    [Required] 
    [Range(0, int.MaxValue)]
    public int Balance { get; set; }
    
    /// <summary>
    /// Navigation property to the <see cref="PointTransaction"/> entity records which added points to <see cref="Balance"/>
    /// </summary>
    [InverseProperty(nameof(PointTransaction.PayerBalance))]
    public ICollection<PointTransaction> PointTransactions 
    {
        get => LazyLoader.Load(this, ref _pointTransactions!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _pointTransactions = value;
    }
    
    /// <summary>
    /// Navigation property to the <see cref="AvailablePoints"/> entity records which track the allocated and unallocated points by the <see cref="PointTransaction"/> entity record where they originated.
    /// </summary>
    [InverseProperty(nameof(EntityModel.AvailablePoints.PayerBalance))]
    public ICollection<AvailablePoints> AvailablePoints
    {
        get => LazyLoader.Load(this, ref _availablePoints!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _availablePoints = value;
    }
    
    /// <summary>
    /// Navigation property to the <see cref="AllocatedPoints"/> entity records which subtracted points from <see cref="Balance"/>
    /// </summary>
    [InverseProperty(nameof(EntityModel.AllocatedPoints.PayerBalance))]
    public ICollection<AllocatedPoints> AllocatedPoints 
    {
        get => LazyLoader.Load(this, ref _allocatedPoints!)!; 
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        set => _allocatedPoints = value;
    }
}