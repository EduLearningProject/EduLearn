using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("FeeSchedules")]
public class FeeSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FeeID { get; set; }

    [Required]
    public int ProgramID { get; set; }

    [Required]
    [MaxLength(20)]
    public string Term { get; set; } = null!;

    [Required]
    public string FeeItemsJSON { get; set; } = null!;

    [Required]
    [Column(TypeName = "date")]
    public DateTime EffectiveFrom { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime EffectiveTo { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey(nameof(ProgramID))]
    public Program Program { get; set; } = null!;
}
