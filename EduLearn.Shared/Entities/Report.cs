using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Reports")]
public class Report
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReportID { get; set; }

    [Required]
    [MaxLength(50)]
    public string Scope { get; set; } = null!;

    public string? ParametersJSON { get; set; }

    public string? MetricsJSON { get; set; }

    [Required]
    public int GeneratedByFK { get; set; }

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? ReportURI { get; set; }

    // Navigation properties
    [ForeignKey(nameof(GeneratedByFK))]
    public User GeneratedBy { get; set; } = null!;
}
