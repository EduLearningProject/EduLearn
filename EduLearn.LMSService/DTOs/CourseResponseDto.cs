namespace EduLearn.LMSService.DTOs;

public class CourseResponseDto
{
    public int CourseID { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public int? DepartmentID { get; set; }
    public string? Level { get; set; }
    public string? PrerequisitesJSON { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
