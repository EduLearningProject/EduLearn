using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Attendance")]
public class Attendance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(10)]
    public string Status { get; set; } = null!;

    [Required]
    public int MarkedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public User Student { get; set; } = null!;

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    [ForeignKey(nameof(MarkedBy))]
    public User MarkedByFaculty { get; set; } = null!;
}
