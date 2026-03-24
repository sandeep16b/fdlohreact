using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents an asset in the system
    /// </summary>
    [Table("Assets")]
    public class Asset
    {
        [Key]
        public int Id { get; set; }

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
        public bool IsOwnedByCounty { get; set; } = false;

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
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedDate { get; set; }

        [StringLength(50)]
        public string? DeletedBy { get; set; }

        // Navigation properties
        [ForeignKey("ReceivableReportId")]
        public virtual ReceivableReport ReceivableReport { get; set; } = null!;

        [ForeignKey("ObjectCodeId")]
        public virtual ObjectCode? ObjectCodeNavigation { get; set; }

        [ForeignKey("AssetGroupId")]
        public virtual AssetGroup? AssetGroup { get; set; }

        [ForeignKey("AssetSubGroupId")]
        public virtual AssetGroup? AssetSubGroup { get; set; }

        [ForeignKey("CountyId")]
        public virtual County? County { get; set; }
    }
}
