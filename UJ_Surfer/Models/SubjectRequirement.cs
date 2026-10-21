using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UJ_Surfer.Models
{
    public class SubjectRequirement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProgrammeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;
        [Required]
        [StringLength(150)]
        public string Requirement { get; set; } = string.Empty; // e.g., "5 (60%+)", "Not accepted"

        // Navigation property
        [ForeignKey(nameof(ProgrammeId))]
        public virtual Programme Programme { get; set; } = null!;
    }
}
