using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("CourseMaterials")]
public class CourseMaterial
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
    [MaxLength(500)]
    public string FileUrl { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string FileType { get; set; } = null!;

    public int ModuleNumber { get; set; } = 1;

    public int OrderIndex { get; set; } = 0;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;
}
