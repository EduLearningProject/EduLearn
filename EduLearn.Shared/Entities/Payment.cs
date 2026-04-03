using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduLearn.Shared.Entities;

[Table("Payments")]
public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PaymentID { get; set; }

    [Required]
    public int InvoiceID { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(30)]
    public string Method { get; set; } = null!;

    [MaxLength(100)]
    public string? Reference { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Completed";

    // Navigation properties
    [ForeignKey(nameof(InvoiceID))]
    public Invoice Invoice { get; set; } = null!;
}
