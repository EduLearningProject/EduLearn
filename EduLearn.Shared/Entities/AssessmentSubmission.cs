using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("AssessmentSubmissions")]
public class AssessmentSubmission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int AssessmentId { get; set; }

    [Required]
    public int StudentId { get; set; }

    public string? Answers { get; set; }

    [Column(TypeName = "decimal(5,1)")]
    public decimal? Score { get; set; }

    [Column(TypeName = "decimal(5,1)")]
    public decimal? MaxScore { get; set; }

    public bool IsGraded { get; set; } = false;

    [MaxLength(2000)]
    public string? Feedback { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? GradedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(AssessmentId))]
    public Assessment Assessment { get; set; } = null!;

    [ForeignKey(nameof(StudentId))]
    public User Student { get; set; } = null!;
}
