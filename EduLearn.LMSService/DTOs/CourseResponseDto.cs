namespace EduLearn.LMSService.DTOs;

public class CourseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int FacultyId { get; set; }
    public string FacultyName { get; set; } = null!;
    public int Semester { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}
