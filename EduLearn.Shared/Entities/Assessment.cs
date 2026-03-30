using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Assessments")]
public class Assessment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = null!;

    public string? Questions { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,1)")]
    public decimal MaxMarks { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsPublished { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    public ICollection<AssessmentSubmission> Submissions { get; set; } = new List<AssessmentSubmission>();
}
