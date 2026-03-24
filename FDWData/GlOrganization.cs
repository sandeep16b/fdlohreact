using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.FDWData
{
    [Table("GL_Organization", Schema = "PalmMD")]
    public partial class GlOrganization
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("SetID")]
        [StringLength(5)]
        public string? SetID { get; set; }

        [Column("Organization")]
        [StringLength(10)]
        public string Organization { get; set; } = null!;

        [Column("EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [Column("EffectiveStatus")]
        [StringLength(1)]
        public string? EffectiveStatus { get; set; }

        [Column("OrganizationDesc")]
        [StringLength(8000)]
        public string? OrganizationDesc { get; set; }

        [Column("OrganizationShortDesc")]
        [StringLength(10)]
        public string? OrganizationShortDesc { get; set; }

        [Column("OrganizationLongDesc")]
        [StringLength(8000)]
        public string? OrganizationLongDesc { get; set; }

        [Column("FDW_LoadDate")]
        public DateTime? FDWLoadDate { get; set; }

        [Column("FDW_StaleFlag")]
        public bool? FDWStaleFlag { get; set; }

        // Convenience properties for backward compatibility
        [NotMapped]
        public int Id => ID;

        [NotMapped]
        public int OrganizationId => ID;

        [NotMapped]
        public string Code => Organization;

        [NotMapped]
        public string OrganizationCode => Organization;

        [NotMapped]
        public string Name => OrganizationShortDesc ?? OrganizationDesc ?? "";

        [NotMapped]
        public string OrganizationName => OrganizationShortDesc ?? OrganizationDesc ?? "";

        [NotMapped]
        public bool IsActive => EffectiveStatus == "A" || EffectiveStatus == "1";

        [NotMapped]
        public DateTime? CreatedDate => FDWLoadDate;

        [NotMapped]
        public DateTime? ModifiedDate => null;
    }
}
