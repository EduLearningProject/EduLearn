using System.ComponentModel.DataAnnotations;

namespace EduLearn.LMSService.DTOs;

public class CreateCourseDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int FacultyId { get; set; }

    [Required]
    [Range(1, 8)]
    public int Semester { get; set; }

    [Range(1, 500)]
    public int MaxCapacity { get; set; } = 30;
}
