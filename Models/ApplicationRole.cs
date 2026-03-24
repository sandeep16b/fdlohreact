using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents an application role reference table
    /// </summary>
    [Table("ApplicationRoles")]
    public class ApplicationRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<ApplicationSecurity> ApplicationSecurityRecords { get; set; } = new List<ApplicationSecurity>();
    }
}

