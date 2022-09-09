using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetchRewardsApi.EntityModel;

internal class PointTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int PointTransactionId { get; set; }

    [MinLength(3)]
    [MaxLength(50)]
    [Required(AllowEmptyStrings = false)]
    public string Payer { get; set; } = null!;
}