using System.ComponentModel.DataAnnotations;

namespace EduLearn.AuthService.DTOs;

public class UpdateStatusDto
{
    [Required]
    [RegularExpression("^(Active|Inactive|Suspended|Locked)$",
        ErrorMessage = "Status must be Active, Inactive, Suspended, or Locked")]
    public string Status { get; set; } = null!;
}
