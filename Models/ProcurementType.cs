using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Master reference table for procurement types
    /// </summary>
    [Table("ProcurementTypes")]
    public class ProcurementType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public virtual ICollection<ProcurementMethod> ProcurementMethods { get; set; } = new List<ProcurementMethod>();
    }
}
