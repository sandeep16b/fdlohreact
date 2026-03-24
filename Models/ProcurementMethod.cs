using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents procurement method details with reference to procurement type
    /// </summary>
    [Table("ProcurementMethods")]
    public class ProcurementMethod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string ProcurementTypeCode { get; set; } = string.Empty;

        // New ID-based foreign key
        public int? ProcurementTypeId { get; set; }

        public DateTime? ChargeDate { get; set; }

        [StringLength(100)]
        public string? PcardHolderFirstName { get; set; }

        [StringLength(100)]
        public string? PcardHolderLastName { get; set; }

        [StringLength(50)]
        public string? GroupId { get; set; }

        [StringLength(100)]
        public string? PurchaseOrderNumber { get; set; }

        [StringLength(100)]
        public string? ContractNumber { get; set; }

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
        [ForeignKey("ProcurementTypeId")]
        public virtual ProcurementType? ProcurementType { get; set; }

        public virtual ICollection<ReceivableReport> ReceivableReports { get; set; } = new List<ReceivableReport>();
    }
}
