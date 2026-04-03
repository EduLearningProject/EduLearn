using System.ComponentModel.DataAnnotations;

namespace EduLearn.SISService.DTOs;

public class CreateEnrollmentDto
{
    [Required]
    public int StudentID { get; set; }

    [Required]
    public int SectionID { get; set; }
}
