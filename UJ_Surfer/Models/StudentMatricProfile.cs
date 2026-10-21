using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UJ_Surfer.Models
{
    public class StudentMatricProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public int EnglishMark { get; set; }

        [StringLength(50)]
        public string? SecondLanguageName { get; set; }

        [Range(0, 100)]
        public int SecondLanguageMark { get; set; }

        [Required]
        public MathType SelectedMathType { get; set; }

        [Required]
        [Range(0, 100)]
        public int MathMark { get; set; }

        [StringLength(50)]
        public string? Elective1Name { get; set; }
        public int? Elective1Mark { get; set; }

        [StringLength(50)]
        public string? Elective2Name { get; set; }
        public int? Elective2Mark { get; set; }

        [StringLength(50)]
        public string? Elective3Name { get; set; }
        public int? Elective3Mark { get; set; }

        public int? LifeOrientationMark { get; set; }

        public int CalculatedAps { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}