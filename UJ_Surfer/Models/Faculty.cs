using System.ComponentModel.DataAnnotations;

namespace UJ_Surfer.Models
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string Acronym { get; set; } = string.Empty;

        public string? GeneralNotes { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<Programme> Programmes { get; set; } = new List<Programme>();
    }
}