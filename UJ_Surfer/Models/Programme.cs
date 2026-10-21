using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UJ_Surfer.Models
{
    public class Programme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FacultyId { get; set; }

        [Required]
        [StringLength(50)]
        public string QualificationType { get; set; } = string.Empty; // Degree, Diploma, Extended

        [Required]
        [StringLength(250)]
        public string Category { get; set; } = string.Empty;
        [Required]
        [StringLength(30)]
        public string Code { get; set; } = string.Empty; // e.g., B34CAQ, B8BA3Q

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Campus { get; set; } = string.Empty; // APK, DFC, APB, SWC, or combinations

        [Required]
        [StringLength(150)]
        public string MinimumApsText { get; set; } = string.Empty; // Preserves raw string conditions

        public int? MinimumApsValue { get; set; } // Nullable base int for numeric filtering

        [Required]
        public string Career { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(FacultyId))]
        public virtual Faculty Faculty { get; set; } = null!;

        public virtual ICollection<SubjectRequirement> SubjectRequirements { get; set; } = new List<SubjectRequirement>();
    }
}
