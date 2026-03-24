using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents a location in the system
    /// </summary>
    [Table("Locations")]
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string County { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Country { get; set; } = "USA";

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public virtual ICollection<ReceivableReport> ReceivableReports { get; set; } = new List<ReceivableReport>();
    }
}
