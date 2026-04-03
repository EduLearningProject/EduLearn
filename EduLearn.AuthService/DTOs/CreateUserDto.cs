using System.ComponentModel.DataAnnotations;

namespace EduLearn.AuthService.DTOs;

public class CreateUserDto
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    [RegularExpression("^(Student|Instructor|Registrar|DeptAdmin|Finance|ITAdmin|Auditor)$",
        ErrorMessage = "Role must be Student, Instructor, Registrar, DeptAdmin, Finance, ITAdmin, or Auditor")]
    public string Role { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}
