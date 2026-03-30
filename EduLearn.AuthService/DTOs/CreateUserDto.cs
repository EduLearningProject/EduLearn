using System.ComponentModel.DataAnnotations;

namespace EduLearn.AuthService.DTOs;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression("^(Admin|Faculty|Student)$", ErrorMessage = "Role must be Admin, Faculty, or Student")]
    public string Role { get; set; } = null!;

    [MaxLength(100)]
    public string? Department { get; set; }
}
