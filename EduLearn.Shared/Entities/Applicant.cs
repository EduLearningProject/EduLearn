using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Applicants")]
public class Applicant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ApplicantID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [Column(TypeName = "date")]
    public DateTime DOB { get; set; }

    [MaxLength(50)]
    public string? NationalID { get; set; }

    public string? ContactInfoJSON { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProgramApplied { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string ApplicationStatus { get; set; } = "Submitted";

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public string? DocumentsURIJSON { get; set; }
}
