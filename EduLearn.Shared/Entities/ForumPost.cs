using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("ForumPosts")]
public class ForumPost
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    public int? ParentId { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [Required]
    [MaxLength(4000)]
    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    [ForeignKey(nameof(AuthorId))]
    public User Author { get; set; } = null!;

    [ForeignKey(nameof(ParentId))]
    public ForumPost? Parent { get; set; }

    public ICollection<ForumPost> Replies { get; set; } = new List<ForumPost>();
}
