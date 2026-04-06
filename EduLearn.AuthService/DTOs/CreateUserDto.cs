using System.ComponentModel.DataAnnotations;
using EduLearn.Shared.Enums;

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
    public UserRole Role { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}
