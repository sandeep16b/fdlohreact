using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents the history of changes to receivable reports
    /// </summary>
    [Table("ReceivableReportHistory")]
    public class ReceivableReportHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int ReceivableReportId { get; set; }

        [Required]
        [StringLength(20)]
        public string OperationType { get; set; } = string.Empty; // INSERT, UPDATE, DELETE

        // Foreign key references to FDW database tables
        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int FundId { get; set; }

        [Required]
        public int OA1Id { get; set; }

        [StringLength(10)]
        public string? LocationCode { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string RRStatus { get; set; } = string.Empty;

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

        public DateTime? CompletedDate { get; set; }

        [StringLength(50)]
        public string? CompletedBy { get; set; }

        public DateTime? AttestedDate { get; set; }

        [StringLength(50)]
        public string? AttestedBy { get; set; }

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
