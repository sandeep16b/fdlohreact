using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.Models
{
    /// <summary>
    /// Represents an asset group for categorizing assets
    /// </summary>
    [Table("AssetGroups")]
    public class AssetGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<AssetSubgroup> ParentGroups { get; set; } = new List<AssetSubgroup>();
        public virtual ICollection<AssetSubgroup> ChildGroups { get; set; } = new List<AssetSubgroup>();
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}

