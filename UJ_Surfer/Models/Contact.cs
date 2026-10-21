using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UJ_Surfer.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FacultyId { get; set; }

        [StringLength(50)]
        public string? Campus { get; set; }

        [Required]
        [StringLength(100)]
        public string Telephone { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [ForeignKey(nameof(FacultyId))]
        public virtual Faculty Faculty { get; set; } = null!;
    }
}
