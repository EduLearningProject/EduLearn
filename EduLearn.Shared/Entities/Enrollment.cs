using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Enrollments")]
public class Enrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    [MaxLength(2)]
    public string? Grade { get; set; }

    [Column(TypeName = "decimal(3,1)")]
    public decimal? GradePoint { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public User Student { get; set; } = null!;

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;
}
