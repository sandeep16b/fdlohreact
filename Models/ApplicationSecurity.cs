using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents application security user with roles and access control
    /// </summary>
    [Table("ApplicationSecurity")]
    public class ApplicationSecurity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [Required]
        public int ApplicationRoleId { get; set; }

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
        [ForeignKey("ApplicationRoleId")]
        public virtual ApplicationRole ApplicationRole { get; set; } = null!;
    }
}

