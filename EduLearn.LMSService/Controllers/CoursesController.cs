using EduLearn.LMSService.Data;
using EduLearn.LMSService.DTOs;
using EduLearn.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.LMSService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly LMSDbContext _context;

    public CoursesController(LMSDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponseDto>> CreateCourse(CreateCourseDto dto, CancellationToken cancellationToken)
    {
        var faculty = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.FacultyId && u.Role == "Faculty", cancellationToken);

        if (faculty is null)
            return BadRequest(new { error = "Invalid FacultyId or user is not Faculty role", code = "INVALID_FACULTY" });

        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            FacultyId = dto.FacultyId,
            Semester = dto.Semester,
            MaxCapacity = dto.MaxCapacity
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            FacultyId = course.FacultyId,
            FacultyName = $"{faculty.FirstName} {faculty.LastName}",
            Semester = course.Semester,
            MaxCapacity = course.MaxCapacity,
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt
        };

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseResponseDto>>> GetCourses(CancellationToken cancellationToken)
    {
        var courses = await _context.Courses
            .AsNoTracking()
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                FacultyId = c.FacultyId,
                FacultyName = c.Faculty.FirstName + " " + c.Faculty.LastName,
                Semester = c.Semester,
                MaxCapacity = c.MaxCapacity,
                IsPublished = c.IsPublished,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseResponseDto>> GetCourse(int id, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                FacultyId = c.FacultyId,
                FacultyName = c.Faculty.FirstName + " " + c.Faculty.LastName,
                Semester = c.Semester,
                MaxCapacity = c.MaxCapacity,
                IsPublished = c.IsPublished,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
            return NotFound(new { error = "Course not found", code = "COURSE_NOT_FOUND" });

        return Ok(course);
    }
}
