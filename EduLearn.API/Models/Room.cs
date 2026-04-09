using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.API.Models;

[Table("Rooms")]
public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoomID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Building { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string RoomNumber { get; set; } = null!;

    [Required]
    public int Capacity { get; set; }

    public string? ResourcesJSON { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Available";

    // Navigation properties
    public ICollection<Section> Sections { get; set; } = new List<Section>();
}
