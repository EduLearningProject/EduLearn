using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Transcripts")]
public class Transcript
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TranscriptID { get; set; }

    [Required]
    public int StudentID { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string EntriesJSON { get; set; } = null!;

    [Column(TypeName = "decimal(3,2)")]
    public decimal? GPA { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Draft";

    [MaxLength(500)]
    public string? TranscriptURI { get; set; }

    // Navigation properties
    [ForeignKey(nameof(StudentID))]
    public Student Student { get; set; } = null!;
}
