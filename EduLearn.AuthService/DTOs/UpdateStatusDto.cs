using System.ComponentModel.DataAnnotations;
using EduLearn.Shared.Enums;

namespace EduLearn.AuthService.DTOs;

public class UpdateStatusDto
{
    [Required]
    public UserStatus Status { get; set; }
}
