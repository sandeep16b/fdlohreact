using Microsoft.EntityFrameworkCore;

namespace ReactAspNetApp.FDWData
{
    public partial class FDWDbContext : DbContext
    {
        public FDWDbContext(DbContextOptions<FDWDbContext> options) : base(options)
        {
        }

        public virtual DbSet<GlFund> GL_Funds { get; set; } = null!;
        public virtual DbSet<GlOa1> GL_OA1s { get; set; } = null!;
        public virtual DbSet<GlOrganization> GL_Organizations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GlFund>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.ToTable("GL_Fund", "PalmMD");
            });

            modelBuilder.Entity<GlOa1>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.ToTable("GL_OA1", "PalmMD");
            });

            modelBuilder.Entity<GlOrganization>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.ToTable("GL_Organization", "PalmMD");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

