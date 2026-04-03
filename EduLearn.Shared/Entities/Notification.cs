using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Notifications")]
public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NotificationID { get; set; }

    [Required]
    public int UserID { get; set; }

    public int? EntityID { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string Category { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string Severity { get; set; } = "Info";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey(nameof(UserID))]
    public User User { get; set; } = null!;
}
