using EduLearn.AuthService.Data;
using EduLearn.AuthService.DTOs;
using EduLearn.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AuthDbContext _context;

    public UsersController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email, cancellationToken))
            return Conflict(new { error = "Email already exists", code = "DUPLICATE_EMAIL" });

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = dto.Password, // Plain text for now — BCrypt later
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = dto.Role,
            Department = dto.Department
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var response = MapToDto(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                Department = u.Department,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
            return NotFound(new { error = "User not found", code = "USER_NOT_FOUND" });

        return Ok(MapToDto(user));
    }

    private static UserResponseDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role,
        Department = user.Department,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
