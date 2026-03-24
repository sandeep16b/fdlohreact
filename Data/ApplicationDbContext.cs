using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.Models;

namespace ReactAspNetApp.Data
{
    /// <summary>
    /// Application database context for Entity Framework
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Main entities
        // Note: Organizations, Funds, and OA1s are in FDW database (accessed via FDWDbContext)
        public DbSet<Location> Locations { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<ObjectCode> ObjectCodes { get; set; }
        public DbSet<ProcurementType> ProcurementTypes { get; set; }
        public DbSet<ProcurementMethod> ProcurementMethods { get; set; }
        public DbSet<ReceivableReport> ReceivableReports { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetGroup> AssetGroups { get; set; }
        public DbSet<AssetSubgroup> AssetSubgroups { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationSecurity> ApplicationSecurity { get; set; }

        // History entities
        public DbSet<ReceivableReportHistory> ReceivableReportHistory { get; set; }
        public DbSet<AssetHistory> AssetHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Note: Organization configuration removed - now in FDW database
            
            // Configure Locations
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).IsFixedLength();
            });

            // Configure Counties
            modelBuilder.Entity<County>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.Name, e.State }).IsUnique();
                entity.Property(e => e.Code).IsFixedLength();
            });

            // Configure ObjectCodes
            modelBuilder.Entity<ObjectCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Code).IsFixedLength();
            });

            // Configure ProcurementTypes
            modelBuilder.Entity<ProcurementType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Code).IsFixedLength();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure ProcurementMethods
            modelBuilder.Entity<ProcurementMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Foreign key relationship
                entity.HasOne(e => e.ProcurementType)
                    .WithMany(pt => pt.ProcurementMethods)
                    .HasForeignKey(e => e.ProcurementTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.ProcurementTypeId);
                entity.HasIndex(e => e.ProcurementTypeCode); // Keep for backward compatibility
                entity.HasIndex(e => e.ChargeDate);
                entity.HasIndex(e => e.PurchaseOrderNumber);
                entity.HasIndex(e => e.ContractNumber);
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure ReceivableReports
            modelBuilder.Entity<ReceivableReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Foreign key relationships
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.ReceivableReports)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ProcurementMethod)
                    .WithMany(pm => pm.ReceivableReports)
                    .HasForeignKey(e => e.ProcurementMethodId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes for foreign key fields
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.FundId);
                entity.HasIndex(e => e.OA1Id);
                entity.HasIndex(e => e.LocationId);
                entity.HasIndex(e => e.LocationCode); // Keep for backward compatibility
                entity.HasIndex(e => e.ProcurementMethodId);
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => e.OrderStatus);
                entity.HasIndex(e => e.IsDeleted);

                // Check constraints
                entity.ToTable(t => t.HasCheckConstraint("CK_ReceivableReport_OrderStatus", 
                    "[OrderStatus] IN ('Partial', 'Complete')"));
            });

            // Configure Assets
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Foreign key relationships
                entity.HasOne(e => e.ReceivableReport)
                    .WithMany(rr => rr.Assets)
                    .HasForeignKey(e => e.ReceivableReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ObjectCodeNavigation)
                    .WithMany(oc => oc.Assets)
                    .HasForeignKey(e => e.ObjectCodeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.County)
                    .WithMany(c => c.Assets)
                    .HasForeignKey(e => e.CountyId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Configure Asset Group relationships
                entity.HasOne(e => e.AssetGroup)
                    .WithMany(ag => ag.Assets)
                    .HasForeignKey(e => e.AssetGroupId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.AssetSubGroup)
                    .WithMany()
                    .HasForeignKey(e => e.AssetSubGroupId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Indexes
                entity.HasIndex(e => e.ReceivableReportId);
                entity.HasIndex(e => e.AssetTag).IsUnique();
                entity.HasIndex(e => e.SerialNumber);
                entity.HasIndex(e => e.ObjectCodeId);
                entity.HasIndex(e => e.AssetGroupId);
                entity.HasIndex(e => e.AssetSubGroupId);
                entity.HasIndex(e => e.CountyId);
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => e.IsDeleted);

                // Decimal precision
                entity.Property(e => e.AssetValue)
                    .HasColumnType("decimal(18,2)");
            });

            // Configure ReceivableReportHistory
            modelBuilder.Entity<ReceivableReportHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId);
                entity.HasIndex(e => e.ReceivableReportId);
                entity.HasIndex(e => e.HistoryDate);
                entity.HasIndex(e => e.OperationType);

                entity.ToTable(t => t.HasCheckConstraint("CK_ReceivableReportHistory_OperationType", 
                    "[OperationType] IN ('INSERT', 'UPDATE', 'DELETE')"));
            });

            // Configure AssetHistory
            modelBuilder.Entity<AssetHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId);
                entity.HasIndex(e => e.AssetId);
                entity.HasIndex(e => e.HistoryDate);
                entity.HasIndex(e => e.OperationType);

                entity.ToTable(t => t.HasCheckConstraint("CK_AssetHistory_OperationType", 
                    "[OperationType] IN ('INSERT', 'UPDATE', 'DELETE')"));

                // Decimal precision
                entity.Property(e => e.AssetValue)
                    .HasColumnType("decimal(18,2)");
            });

            // Configure AssetGroup
            modelBuilder.Entity<AssetGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure AssetSubgroup
            modelBuilder.Entity<AssetSubgroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ParentAssetGroupId, e.ChildAssetGroupId }).IsUnique();
                entity.HasIndex(e => e.ParentAssetGroupId);
                entity.HasIndex(e => e.ChildAssetGroupId);
                entity.HasIndex(e => e.IsActive);

                // Configure relationships
                entity.HasOne(e => e.ParentAssetGroup)
                    .WithMany(p => p.ParentGroups)
                    .HasForeignKey(e => e.ParentAssetGroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ChildAssetGroup)
                    .WithMany(c => c.ChildGroups)
                    .HasForeignKey(e => e.ChildAssetGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ApplicationRole
            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure ApplicationSecurity
            modelBuilder.Entity<ApplicationSecurity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.ApplicationRoleId);
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => e.IsActive);

                // Foreign key relationship
                entity.HasOne(e => e.ApplicationRole)
                    .WithMany(ar => ar.ApplicationSecurityRecords)
                    .HasForeignKey(e => e.ApplicationRoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Note: Seed data temporarily disabled due to identity column conflicts
            // Will be re-enabled after database migration is complete
            
            // TODO: Re-implement seed data using proper identity column handling
            // The seed data needs to be restructured to work with identity columns
            // or implemented through SQL scripts after migration
        }

        /// <summary>
        /// Override SaveChanges to implement automatic history tracking
        /// </summary>
        public override int SaveChanges()
        {
            TrackChanges();
            return base.SaveChanges();
        }

        /// <summary>
        /// Override SaveChangesAsync to implement automatic history tracking
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            TrackChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void TrackChanges()
        {
            var currentUser = "System"; // In a real application, get this from the current user context

            // Snapshot the entries to avoid "Collection was modified" exception
            // when adding history entries during iteration
            var entries = ChangeTracker.Entries().ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity is ReceivableReport rr && entry.State != EntityState.Unchanged)
                {
                    var operationType = entry.State switch
                    {
                        EntityState.Added => "INSERT",
                        EntityState.Modified => "UPDATE",
                        EntityState.Deleted => "DELETE",
                        _ => "UPDATE"
                    };

                    var historyEntry = new ReceivableReportHistory
                    {
                        ReceivableReportId = rr.Id,
                        OperationType = operationType,
                        OrganizationId = rr.OrganizationId,
                        FundId = rr.FundId,
                        OA1Id = rr.OA1Id,
                        LocationCode = rr.LocationCode,
                        OrderStatus = rr.OrderStatus,
                        RRStatus = rr.RRStatus,
                        AddressLine1 = rr.AddressLine1,
                        AddressLine2 = rr.AddressLine2,
                        City = rr.City,
                        County = rr.County,
                        State = rr.State,
                        PostalCode = rr.PostalCode,
                        ProcurementMethodId = rr.ProcurementMethodId,
                        CompletedDate = rr.CompletedDate,
                        CompletedBy = rr.CompletedBy,
                        AttestedDate = rr.AttestedDate,
                        AttestedBy = rr.AttestedBy,
                        IsDeleted = rr.IsDeleted,
                        DeletedDate = rr.DeletedDate,
                        DeletedBy = rr.DeletedBy,
                        HistoryUser = currentUser,
                        OriginalCreatedDate = rr.CreatedDate,
                        OriginalCreatedBy = rr.CreatedBy,
                        OriginalModifiedDate = rr.ModifiedDate,
                        OriginalModifiedBy = rr.ModifiedBy
                    };

                    ReceivableReportHistory.Add(historyEntry);
                }

                if (entry.Entity is Asset asset && entry.State != EntityState.Unchanged)
                {
                    var operationType = entry.State switch
                    {
                        EntityState.Added => "INSERT",
                        EntityState.Modified => "UPDATE",
                        EntityState.Deleted => "DELETE",
                        _ => "UPDATE"
                    };

                    var historyEntry = new AssetHistory
                    {
                        AssetId = asset.Id,
                        OperationType = operationType,
                        ReceivableReportId = asset.ReceivableReportId,
                        Brand = asset.Brand,
                        Make = asset.Make,
                        Model = asset.Model,
                        AssetTag = asset.AssetTag,
                        SerialNumber = asset.SerialNumber,
                        AssetValue = asset.AssetValue,
                        ObjectCodeId = asset.ObjectCodeId,
                        AssetGroupId = asset.AssetGroupId,
                        AssetSubGroupId = asset.AssetSubGroupId,
                        AssignedTo = asset.AssignedTo,
                        Floor = asset.Floor,
                        Room = asset.Room,
                        IsOwnedByCounty = asset.IsOwnedByCounty,
                        CountyId = asset.CountyId,
                        UniqueTagNumber = asset.UniqueTagNumber,
                        AssetStatus = asset.AssetStatus,
                        TagPrintedDate = asset.TagPrintedDate,
                        TagPrintedBy = asset.TagPrintedBy,
                        TagAttestedDate = asset.TagAttestedDate,
                        TagAttestedBy = asset.TagAttestedBy,
                        IsDeleted = asset.IsDeleted,
                        DeletedDate = asset.DeletedDate,
                        DeletedBy = asset.DeletedBy,
                        HistoryUser = currentUser,
                        OriginalCreatedDate = asset.CreatedDate,
                        OriginalCreatedBy = asset.CreatedBy,
                        OriginalModifiedDate = asset.ModifiedDate,
                        OriginalModifiedBy = asset.ModifiedBy
                    };

                    AssetHistory.Add(historyEntry);
                }

                // Update modification timestamps
                if (entry.Entity is ReceivableReport rrEntity && entry.State == EntityState.Modified)
                {
                    rrEntity.ModifiedDate = DateTime.UtcNow;
                    rrEntity.ModifiedBy = currentUser;
                }

                if (entry.Entity is Asset assetEntity && entry.State == EntityState.Modified)
                {
                    assetEntity.ModifiedDate = DateTime.UtcNow;
                    assetEntity.ModifiedBy = currentUser;
                }
            }
        }
    }
}
