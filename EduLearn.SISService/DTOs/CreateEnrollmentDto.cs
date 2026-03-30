using System.ComponentModel.DataAnnotations;

namespace EduLearn.SISService.DTOs;

public class CreateEnrollmentDto
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }
}
