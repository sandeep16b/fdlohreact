using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.Data;
using ReactAspNetApp.FDWData;

namespace ReactAspNetApp.Services
{
    /// <summary>
    /// Service for initializing the database with migrations and seed data
    /// </summary>
    public class DatabaseInitializationService
    {
        private readonly ApplicationDbContext _context;
        private readonly FDWDbContext _fdwContext;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(
            ApplicationDbContext context, 
            FDWDbContext fdwContext,
            ILogger<DatabaseInitializationService> logger)
        {
            _context = context;
            _fdwContext = fdwContext;
            _logger = logger;
        }

        /// <summary>
        /// Initialize the database by applying migrations and ensuring seed data
        /// </summary>
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Starting database initialization...");

                // Apply any pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("Database migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations found.");
                }

                // Ensure the database is created
                await _context.Database.EnsureCreatedAsync();

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        /// <summary>
        /// Check if the database has been seeded with initial data
        /// </summary>
        public async Task<bool> IsDatabaseSeededAsync()
        {
            try
            {
                // Check FDW database for Organizations (reference data)
                var hasOrganizations = await _fdwContext.GL_Organizations.AnyAsync();
                
                // Check local database for other data
                return hasOrganizations &&
                       await _context.Locations.AnyAsync() &&
                       await _context.Counties.AnyAsync() &&
                       await _context.ObjectCodes.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if database is seeded.");
                return false;
            }
        }

        /// <summary>
        /// Get database statistics for logging/monitoring
        /// </summary>
        public async Task<DatabaseStatistics> GetDatabaseStatisticsAsync()
        {
            try
            {
                var stats = new DatabaseStatistics
                {
                    // FDW database counts (reference data)
                    OrganizationCount = await _fdwContext.GL_Organizations.CountAsync(),
                    FundCount = await _fdwContext.GL_Funds.CountAsync(),
                    OA1Count = await _fdwContext.GL_OA1s.CountAsync(),
                    
                    // Local database counts
                    LocationCount = await _context.Locations.CountAsync(),
                    CountyCount = await _context.Counties.CountAsync(),
                    ObjectCodeCount = await _context.ObjectCodes.CountAsync(),
                    ReceivableReportCount = await _context.ReceivableReports.Where(rr => !rr.IsDeleted).CountAsync(),
                    AssetCount = await _context.Assets.Where(a => !a.IsDeleted).CountAsync(),
                    ReceivableReportHistoryCount = await _context.ReceivableReportHistory.CountAsync(),
                    AssetHistoryCount = await _context.AssetHistory.CountAsync()
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database statistics.");
                throw;
            }
        }
    }

    /// <summary>
    /// Database statistics for monitoring
    /// </summary>
    public class DatabaseStatistics
    {
        // FDW database counts (reference data)
        public int OrganizationCount { get; set; }
        public int FundCount { get; set; }
        public int OA1Count { get; set; }
        
        // Local database counts
        public int LocationCount { get; set; }
        public int CountyCount { get; set; }
        public int ObjectCodeCount { get; set; }
        public int ReceivableReportCount { get; set; }
        public int AssetCount { get; set; }
        public int ReceivableReportHistoryCount { get; set; }
        public int AssetHistoryCount { get; set; }

        public override string ToString()
        {
            return $"FDW: Organizations: {OrganizationCount}, Funds: {FundCount}, OA1s: {OA1Count} | " +
                   $"Local: Locations: {LocationCount}, Counties: {CountyCount}, Object Codes: {ObjectCodeCount}, " +
                   $"Receivable Reports: {ReceivableReportCount}, Assets: {AssetCount}, " +
                   $"RR History: {ReceivableReportHistoryCount}, Asset History: {AssetHistoryCount}";
        }
    }
}


