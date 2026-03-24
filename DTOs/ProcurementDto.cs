namespace ReactAspNetApp.DTOs
{
    /// <summary>
    /// Data Transfer Object for ProcurementType
    /// </summary>
    public class ProcurementTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for ProcurementMethod
    /// </summary>
    public class ProcurementMethodDto
    {
        public int Id { get; set; }
        public int? ProcurementTypeId { get; set; }
        public string ProcurementTypeCode { get; set; } = string.Empty;
        public string ProcurementTypeName { get; set; } = string.Empty;
        public DateTime? ChargeDate { get; set; }
        public string? PcardHolderFirstName { get; set; }
        public string? PcardHolderLastName { get; set; }
        public string? GroupId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public string? ContractNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for creating/updating ProcurementMethod
    /// </summary>
    public class ProcurementMethodCreateUpdateDto
    {
        public int? ProcurementTypeId { get; set; }
        public string ProcurementTypeCode { get; set; } = string.Empty;
        public DateTime? ChargeDate { get; set; }
        public string? PcardHolderFirstName { get; set; }
        public string? PcardHolderLastName { get; set; }
        public string? GroupId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public string? ContractNumber { get; set; }
    }
}
