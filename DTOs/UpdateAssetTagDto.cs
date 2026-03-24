using System.ComponentModel.DataAnnotations;

namespace ReactAspNetApp.DTOs
{
    /// <summary>
    /// DTO for updating an asset's tag
    /// </summary>
    public class UpdateAssetTagDto
    {
        /// <summary>
        /// Asset tag value. Can be null to reset/clear the tag.
        /// </summary>
        [StringLength(50, ErrorMessage = "Asset tag cannot exceed 50 characters")]
        public string? AssetTag { get; set; }
    }
}

