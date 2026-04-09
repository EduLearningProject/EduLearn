using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.API.Models;

[Table("Programs")]
public class Program
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProgramID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    public int? DepartmentID { get; set; }

    [Required]
    [MaxLength(50)]
    public string DegreeType { get; set; } = null!;

    public string? RequiredCoursesJSON { get; set; }

    public string? ElectivesJSON { get; set; }

    [Required]
    public int DurationTerms { get; set; } = 8;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    // Navigation properties
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<FeeSchedule> FeeSchedules { get; set; } = new List<FeeSchedule>();
}
