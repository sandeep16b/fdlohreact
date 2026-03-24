using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents a parent-child relationship between asset groups
    /// </summary>
    [Table("AssetSubgroups")]
    public class AssetSubgroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ParentAssetGroupId { get; set; }

        [Required]
        public int ChildAssetGroupId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ParentAssetGroupId")]
        public virtual AssetGroup ParentAssetGroup { get; set; } = null!;

        [ForeignKey("ChildAssetGroupId")]
        public virtual AssetGroup ChildAssetGroup { get; set; } = null!;
    }
}

