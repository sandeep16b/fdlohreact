namespace ReactAspNetApp.DTOs
{
    /// <summary>
    /// Data Transfer Object for ReceivableReport
    /// </summary>
    public class ReceivableReportDto
    {
        public int Id { get; set; }
        
        // Foreign key references to FDW database tables
        public int OrganizationId { get; set; }
        public string OrganizationCode { get; set; } = string.Empty; // From FDW lookup
        public string OrganizationName { get; set; } = string.Empty; // From FDW lookup
        
        public int FundId { get; set; }
        public string FundCode { get; set; } = string.Empty; // From FDW lookup
        public string FundName { get; set; } = string.Empty; // From FDW lookup
        
        public int OA1Id { get; set; }
        public string OA1Code { get; set; } = string.Empty; // From FDW lookup
        public string OA1Name { get; set; } = string.Empty; // From FDW lookup

        
        public int LocationId { get; set; }
        public string? LocationCode { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string RRStatus { get; set; } = string.Empty;
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        
        // Procurement method
        public int? ProcurementMethodId { get; set; }
        public ProcurementMethodDto? ProcurementMethod { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? CompletedBy { get; set; }
        public DateTime? AttestedDate { get; set; }
        public string? AttestedBy { get; set; }
        public List<AssetDto> Assets { get; set; } = new List<AssetDto>();
        public int AssetCount { get; set; }
        public decimal TotalAssetValue { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for creating/updating ReceivableReport
    /// </summary>
    public class ReceivableReportCreateUpdateDto
    {
        // Foreign key references to FDW database tables
        public int OrganizationId { get; set; }
        public int FundId { get; set; }
        public int OA1Id { get; set; }
        
        public int LocationId { get; set; }
        public string? LocationCode { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string RRStatus { get; set; } = string.Empty;
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        
        // Procurement method
        public ProcurementMethodCreateUpdateDto? ProcurementMethod { get; set; }
        
        public List<AssetCreateUpdateDto> Assets { get; set; } = new List<AssetCreateUpdateDto>();
    }

    /// <summary>
    /// Data Transfer Object for searching ReceivableReports
    /// </summary>
    public class ReceivableReportSearchDto
    {
        // Can search by FDW foreign keys or codes
        public int? OrganizationId { get; set; }
        public string? OrganizationCode { get; set; }
        public int? FundId { get; set; }
        public string? FundCode { get; set; }
        public int? OA1Id { get; set; }
        public string? OA1Code { get; set; }
        
        public int LocationId { get; set; }
        public string? LocationCode { get; set; }
        public string? OrderStatus { get; set; }
        public string? RRStatus { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public string? CreatedBy { get; set; }
        public bool IncludeDeleted { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortDirection { get; set; } = "DESC";
    }
}
