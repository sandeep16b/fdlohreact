namespace ReactAspNetApp.DTOs
{
    /// <summary>
    /// Data Transfer Object for Asset
    /// </summary>
    public class AssetDto
    {
        public int Id { get; set; }
        public int ReceivableReportId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string AssetTag { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public decimal AssetValue { get; set; }
        public int? ObjectCodeId { get; set; }
        public string? ObjectCodeName { get; set; }
        public int? AssetGroupId { get; set; }
        public string? AssetGroupName { get; set; }
        public int? AssetSubGroupId { get; set; }
        public string? AssetSubGroupName { get; set; }
        public string? AssignedTo { get; set; }
        public string? Floor { get; set; }
        public string? Room { get; set; }
        public bool IsOwnedByCounty { get; set; }
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public string? UniqueTagNumber { get; set; }
        public string AssetStatus { get; set; } = "Open";
        public DateTime? TagPrintedDate { get; set; }
        public string? TagPrintedBy { get; set; }
        public DateTime? TagAttestedDate { get; set; }
        public string? TagAttestedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for creating/updating Asset
    /// </summary>
    public class AssetCreateUpdateDto
    {
        public int Id { get; set; } // 0 for new assets
        public string Brand { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string AssetTag { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public decimal AssetValue { get; set; }
        public int? ObjectCodeId { get; set; }
        public int? AssetGroupId { get; set; }
        public int? AssetSubGroupId { get; set; }
        public string? AssignedTo { get; set; }
        public string? Floor { get; set; }
        public string? Room { get; set; }
        public bool IsOwnedByCounty { get; set; }
        public int? CountyId { get; set; }
        public string? UniqueTagNumber { get; set; }
        public string AssetStatus { get; set; } = "Open";
    }
}
