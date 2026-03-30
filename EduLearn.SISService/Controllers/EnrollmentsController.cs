using EduLearn.SISService.Data;
using EduLearn.SISService.DTOs;
using EduLearn.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.SISService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly SISDbContext _context;

    public EnrollmentsController(SISDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> CreateEnrollment(CreateEnrollmentDto dto, CancellationToken cancellationToken)
    {
        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.StudentId && u.Role == "Student", cancellationToken);

        if (student is null)
            return BadRequest(new { error = "Invalid StudentId or user is not Student role", code = "INVALID_STUDENT" });

        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CourseId, cancellationToken);

        if (course is null)
            return BadRequest(new { error = "Course not found", code = "COURSE_NOT_FOUND" });

        if (await _context.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId, cancellationToken))
            return Conflict(new { error = "Student is already enrolled in this course", code = "DUPLICATE_ENROLLMENT" });

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            Status = "Active"
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseId = enrollment.CourseId,
            CourseName = course.Title,
            Status = enrollment.Status,
            EnrolledAt = enrollment.EnrolledAt
        };

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<EnrollmentResponseDto>>> GetEnrollments(CancellationToken cancellationToken)
    {
        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Select(e => new EnrollmentResponseDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student.FirstName + " " + e.Student.LastName,
                CourseId = e.CourseId,
                CourseName = e.Course.Title,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt,
                Grade = e.Grade,
                GradePoint = e.GradePoint,
                CompletedAt = e.CompletedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(enrollments);
    }
}
