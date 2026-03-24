using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactAspNetApp.FDWData
{
    [Table("GL_Fund", Schema = "PalmMD")]
    public partial class GlFund
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("SetID")]
        [StringLength(5)]
        public string? SetID { get; set; }

        [Column("Fund")]
        [StringLength(10)]
        public string Fund { get; set; } = null!;

        [Column("EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [Column("EffectiveStatus")]
        [StringLength(1)]
        public string? EffectiveStatus { get; set; }

        [Column("FundDesc")]
        [StringLength(8000)]
        public string? FundDesc { get; set; }

        [Column("FundShortDesc")]
        [StringLength(10)]
        public string? FundShortDesc { get; set; }

        [Column("FundLongDesc")]
        [StringLength(8000)]
        public string? FundLongDesc { get; set; }

        [Column("FDW_LoadDate")]
        public DateTime? FDWLoadDate { get; set; }

        [Column("FDW_StaleFlag")]
        public bool? FDWStaleFlag { get; set; }

        // Convenience properties for backward compatibility
        [NotMapped]
        public int Id => ID;

        [NotMapped]
        public int FundId => ID;

        [NotMapped]
        public string Code => Fund;

        [NotMapped]
        public string FundCode => Fund;

        [NotMapped]
        public string Name => FundShortDesc ?? FundDesc ?? "";

        [NotMapped]
        public string FundName => FundShortDesc ?? FundDesc ?? "";

        [NotMapped]
        public bool IsActive => EffectiveStatus == "A" || EffectiveStatus == "1";

        [NotMapped]
        public DateTime? CreatedDate => FDWLoadDate;

        [NotMapped]
        public DateTime? ModifiedDate => null;
    }
}
