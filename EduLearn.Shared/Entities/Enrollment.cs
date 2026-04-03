using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Enrollments")]
public class Enrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EnrollID { get; set; }

    [Required]
    public int StudentID { get; set; }

    [Required]
    public int SectionID { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Enrolled";

    public int? WaitlistPosition { get; set; }

    public bool GradePostedFlag { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(StudentID))]
    public Student Student { get; set; } = null!;

    [ForeignKey(nameof(SectionID))]
    public Section Section { get; set; } = null!;
}
