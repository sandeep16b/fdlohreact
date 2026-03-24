namespace ReactAspNetApp.DTOs
{
    /// <summary>
    /// Data Transfer Object for lookup data used in dropdowns
    /// </summary>
    public class LookupDataDto
    {
        public object Organizations { get; set; } = new object();
        public object Funds { get; set; } = new object();
        public object OA1s { get; set; } = new object();
        public List<LocationLookupDto> Locations { get; set; } = new List<LocationLookupDto>();
        public List<CountyLookupDto> Counties { get; set; } = new List<CountyLookupDto>();
        public List<ObjectCodeLookupDto> ObjectCodes { get; set; } = new List<ObjectCodeLookupDto>();
        public List<ProcurementTypeLookupDto> ProcurementTypes { get; set; } = new List<ProcurementTypeLookupDto>();
        public List<AssetGroupLookupDto> AssetGroups { get; set; } = new List<AssetGroupLookupDto>();
    }

    /// <summary>
    /// DTO for Location lookup data
    /// </summary>
    public class LocationLookupDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? AddressLine1 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }

    /// <summary>
    /// DTO for County lookup data
    /// </summary>
    public class CountyLookupDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? State { get; set; }
    }

    /// <summary>
    /// DTO for ObjectCode lookup data
    /// </summary>
    public class ObjectCodeLookupDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for ProcurementType lookup data
    /// </summary>
    public class ProcurementTypeLookupDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO for AssetGroup lookup data
    /// </summary>
    public class AssetGroupLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
