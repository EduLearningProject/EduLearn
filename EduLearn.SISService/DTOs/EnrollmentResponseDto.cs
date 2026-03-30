namespace EduLearn.SISService.DTOs;

public class EnrollmentResponseDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime EnrolledAt { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public DateTime? CompletedAt { get; set; }
}
