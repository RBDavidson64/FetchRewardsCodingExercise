using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FetchRewardsApi.EntityModel;

/// <summary>
/// Records how many points were spent and the <see cref="AllocatedPoints"/>
/// entity records detailing which transactions created the points originally and which payers the
/// spent points were allocated to.
/// </summary>
internal sealed record SpentPoints
{
    private ICollection<AllocatedPoints> _allocatedPoints = null!;
    public SpentPoints(){}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lazyLoader">Passed by the <see cref="DbContext"/> to enable lazy-loading of navigation properties.</param>
    public SpentPoints(ILazyLoader lazyLoader) => LazyLoader = lazyLoader;
    
    /// <summary>
    /// Used to lazy-load navigation properties.
    /// </summary>
    private ILazyLoader LazyLoader { get; } = null!;
    
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SpentPointsId { get; init; }

    /// <summary>
    /// The points which were spent.
    /// </summary>
    [Required]
    public int PointsSpent { get; init; }

    /// <summary>
    /// When the points were allocated.
    /// </summary>
    [Required]
    public DateTime TimeStamp { get; init; }
    
    /// <summary>
    /// Navigation property to the <see cref="AllocatedPoints"/> entity records which record how the spent points were allocated.
    /// </summary>
    
    [InverseProperty(nameof(EntityModel.AllocatedPoints.SpentPoints))]
    public ICollection<AllocatedPoints> AllocatedPoints 
    {
        get => LazyLoader.Load(this, ref _allocatedPoints!)!; 
        set => _allocatedPoints = value;
    }
}