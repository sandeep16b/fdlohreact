using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents a receivable report in the system
    /// </summary>
    [Table("ReceivableReports")]
    public class ReceivableReport
    {
        [Key]
        public int Id { get; set; }

        // Foreign key references to FDW database tables
        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int FundId { get; set; }

        [Required]
        public int OA1Id { get; set; }

        [StringLength(10)]
        public string? LocationCode { get; set; }

        // New ID-based foreign key
        public int LocationId { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderStatus { get; set; } = string.Empty; // Partial, Complete

        [Required]
        [StringLength(20)]
        public string RRStatus { get; set; } = "Draft"; // Draft, Submitted, Complete

        [StringLength(200)]
        public string? AddressLine1 { get; set; }

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? County { get; set; }

        [StringLength(10)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        // Procurement method reference
        public int? ProcurementMethodId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        public DateTime? CompletedDate { get; set; }

        [StringLength(50)]
        public string? CompletedBy { get; set; }

        public DateTime? AttestedDate { get; set; }

        [StringLength(50)]
        public string? AttestedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedDate { get; set; }

        [StringLength(50)]
        public string? DeletedBy { get; set; }

        // Navigation properties
        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }

        [ForeignKey("ProcurementMethodId")]
        public virtual ProcurementMethod? ProcurementMethod { get; set; }

        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();

        // Computed properties
        [NotMapped]
        public int AssetCount => Assets?.Count ?? 0;

        [NotMapped]
        public decimal TotalAssetValue => Assets?.Sum(a => a.AssetValue) ?? 0;
    }
}
