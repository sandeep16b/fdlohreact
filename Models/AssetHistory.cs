using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents the history of changes to assets
    /// </summary>
    [Table("AssetHistory")]
    public class AssetHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int AssetId { get; set; }

        [Required]
        [StringLength(20)]
        public string OperationType { get; set; } = string.Empty; // INSERT, UPDATE, DELETE

        [Required]
        public int ReceivableReportId { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [StringLength(50)]
        public string? AssetTag { get; set; }

        [Required]
        [StringLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AssetValue { get; set; }

        // ID-based foreign key for ObjectCode
        public int? ObjectCodeId { get; set; }

        // ID-based foreign keys for Asset Groups
        public int? AssetGroupId { get; set; }
        
        public int? AssetSubGroupId { get; set; }

        [StringLength(100)]
        public string? AssignedTo { get; set; }

        [StringLength(10)]
        public string? Floor { get; set; }

        [StringLength(20)]
        public string? Room { get; set; }

        [Required]
        public bool IsOwnedByCounty { get; set; }

        // ID-based foreign key for County
        public int? CountyId { get; set; }

        [StringLength(50)]
        public string? UniqueTagNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string AssetStatus { get; set; } = "Open";

        public DateTime? TagPrintedDate { get; set; }

        [StringLength(50)]
        public string? TagPrintedBy { get; set; }

        public DateTime? TagAttestedDate { get; set; }

        [StringLength(50)]
        public string? TagAttestedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        [StringLength(50)]
        public string? DeletedBy { get; set; }

        // History tracking fields
        [Required]
        public DateTime HistoryDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string HistoryUser { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ChangeReason { get; set; }

        // Original record tracking
        public DateTime OriginalCreatedDate { get; set; }

        [StringLength(50)]
        public string OriginalCreatedBy { get; set; } = string.Empty;

        public DateTime? OriginalModifiedDate { get; set; }

        [StringLength(50)]
        public string? OriginalModifiedBy { get; set; }
    }
}
