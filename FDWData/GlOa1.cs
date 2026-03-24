using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.FDWData
{
    [Table("GL_OA1", Schema = "PalmMD")]
    public partial class GlOa1
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("SetID")]
        [StringLength(5)]
        public string? SetID { get; set; }

        [Column("OA1")]
        [StringLength(10)]
        public string OA1 { get; set; } = null!;

        [Column("EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [Column("EffectiveStatus")]
        [StringLength(1)]
        public string? EffectiveStatus { get; set; }

        [Column("OA1Desc")]
        [StringLength(8000)]
        public string? OA1Desc { get; set; }

        [Column("OA1ShortDesc")]
        [StringLength(10)]
        public string? OA1ShortDesc { get; set; }

        [Column("OA1LongDesc")]
        [StringLength(8000)]
        public string? OA1LongDesc { get; set; }

        [Column("FDW_LoadDate")]
        public DateTime? FDWLoadDate { get; set; }

        [Column("FDW_StaleFlag")]
        public bool? FDWStaleFlag { get; set; }

        // Convenience properties for backward compatibility
        [NotMapped]
        public int Id => ID;

        [NotMapped]
        public int Oa1Id => ID;

        [NotMapped]
        public string Code => OA1;

        [NotMapped]
        public string Oa1Code => OA1;

        [NotMapped]
        public string Name => OA1ShortDesc ?? OA1Desc ?? "";

        [NotMapped]
        public string Oa1Name => OA1ShortDesc ?? OA1Desc ?? "";

        [NotMapped]
        public bool IsActive => EffectiveStatus == "A" || EffectiveStatus == "1";

        [NotMapped]
        public DateTime? CreatedDate => FDWLoadDate;

        [NotMapped]
        public DateTime? ModifiedDate => null;
    }
}
