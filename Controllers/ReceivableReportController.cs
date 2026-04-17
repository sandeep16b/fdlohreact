using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ReactAspNetApp.Data;
using ReactAspNetApp.FDWData;
using ReactAspNetApp.Models;
using ReactAspNetApp.DTOs;
using ReactAspNetApp.Services;
using System.Data.Common;
using System.Linq;
using System;

namespace ReactAspNetApp.Controllers
{
    /// <summary>
    /// API Controller for managing Receivable Reports
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReceivableReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FDWDbContext _fdwContext;
        private readonly ILogger<ReceivableReportController> _logger;
        private readonly ILoggingService _loggingService;

        public ReceivableReportController(ApplicationDbContext context, FDWDbContext fdwContext, ILogger<ReceivableReportController> logger, ILoggingService loggingService)
        {
            _context = context;
            _fdwContext = fdwContext;
            _logger = logger;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Search for receivable reports with filtering and pagination
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<PagedResult<ReceivableReportDto>>> SearchReceivableReports([FromBody] ReceivableReportSearchDto? searchDto)
        {
            if (searchDto == null)
                return BadRequest("Search parameters are required");

            _loggingService.LogMethodEntry($"PageNumber: {searchDto.PageNumber}, PageSize: {searchDto.PageSize}");

            try
            {
                _loggingService.LogInformation($"Searching receivable reports with filters - PageNumber: {searchDto.PageNumber}, PageSize: {searchDto.PageSize}, OrganizationId: {searchDto.OrganizationId}, FundId: {searchDto.FundId}");
                var query = _context.ReceivableReports
                    .Include(rr => rr.Location)
                    .Include(rr => rr.ProcurementMethod!)
                        .ThenInclude(pm => pm.ProcurementType)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ObjectCodeNavigation!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetSubGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.County!)
                    .AsQueryable();

                // Apply filters
                if (!searchDto.IncludeDeleted)
                {
                    query = query.Where(rr => !rr.IsDeleted);
                }

                // Filter by Organization ID or Code
                if (searchDto.OrganizationId.HasValue)
                {
                    query = query.Where(rr => rr.OrganizationId == searchDto.OrganizationId.Value);
                }
                else if (!string.IsNullOrEmpty(searchDto.OrganizationCode))
                {
                    // Need to lookup OrganizationId from FDW database (use actual column name)
                    var org = await _fdwContext.GL_Organizations
                        .Where(o => o.Organization == searchDto.OrganizationCode && 
                                   (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"))
                        .FirstOrDefaultAsync();
                    if (org != null)
                    {
                        query = query.Where(rr => rr.OrganizationId == org.ID);
                    }
                    else
                    {
                        // If organization code not found, return empty result
                        query = query.Where(rr => false);
                    }
                }

                // Filter by Fund ID or Code
                if (searchDto.FundId.HasValue)
                {
                    query = query.Where(rr => rr.FundId == searchDto.FundId.Value);
                }
                else if (!string.IsNullOrEmpty(searchDto.FundCode))
                {
                    var fund = await _fdwContext.GL_Funds
                        .Where(f => f.Fund == searchDto.FundCode && 
                                   (f.EffectiveStatus == "A" || f.EffectiveStatus == "1"))
                        .FirstOrDefaultAsync();
                    if (fund != null)
                    {
                        query = query.Where(rr => rr.FundId == fund.ID);
                    }
                    else
                    {
                        query = query.Where(rr => false);
                    }
                }

                // Filter by OA1 ID or Code
                if (searchDto.OA1Id.HasValue)
                {
                    query = query.Where(rr => rr.OA1Id == searchDto.OA1Id.Value);
                }
                else if (!string.IsNullOrEmpty(searchDto.OA1Code))
                {
                    var oa1 = await _fdwContext.GL_OA1s
                        .Where(o => o.OA1 == searchDto.OA1Code && 
                                   (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"))
                        .FirstOrDefaultAsync();
                    if (oa1 != null)
                    {
                        query = query.Where(rr => rr.OA1Id == oa1.ID);
                    }
                    else
                    {
                        query = query.Where(rr => false);
                    }
                }

                if (!string.IsNullOrEmpty(searchDto.LocationCode))
                {
                    query = query.Where(rr => rr.LocationCode == searchDto.LocationCode);
                }

                if (!string.IsNullOrEmpty(searchDto.OrderStatus))
                {
                    query = query.Where(rr => rr.OrderStatus == searchDto.OrderStatus);
                }

                if (!string.IsNullOrEmpty(searchDto.RRStatus))
                {
                    query = query.Where(rr => rr.RRStatus == searchDto.RRStatus);
                }

                if (searchDto.CreatedDateFrom.HasValue)
                {
                    query = query.Where(rr => rr.CreatedDate >= searchDto.CreatedDateFrom.Value);
                }

                if (searchDto.CreatedDateTo.HasValue)
                {
                    query = query.Where(rr => rr.CreatedDate <= searchDto.CreatedDateTo.Value);
                }

                if (!string.IsNullOrEmpty(searchDto.CreatedBy))
                {
                    query = query.Where(rr => rr.CreatedBy.Contains(searchDto.CreatedBy));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = searchDto.SortBy.ToLower() switch
                {
                    "organizationid" => searchDto.SortDirection.ToUpper() == "ASC" 
                        ? query.OrderBy(rr => rr.OrganizationId)
                        : query.OrderByDescending(rr => rr.OrganizationId),
                    "fundid" => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.FundId)
                        : query.OrderByDescending(rr => rr.FundId),
                    "oa1id" => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.OA1Id)
                        : query.OrderByDescending(rr => rr.OA1Id),
                    "locationcode" => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.LocationCode)
                        : query.OrderByDescending(rr => rr.LocationCode),
                    "orderstatus" => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.OrderStatus)
                        : query.OrderByDescending(rr => rr.OrderStatus),
                    "rrstatus" => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.RRStatus)
                        : query.OrderByDescending(rr => rr.RRStatus),
                    "createdby" => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.CreatedBy)
                        : query.OrderByDescending(rr => rr.CreatedBy),
                    _ => searchDto.SortDirection.ToUpper() == "ASC"
                        ? query.OrderBy(rr => rr.CreatedDate)
                        : query.OrderByDescending(rr => rr.CreatedDate)
                };

                // Apply pagination
                var items = await query
                    .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToListAsync();

                // Map to DTOs
                var dtos = await MapToDtoListAsync(items);

                var result = new PagedResult<ReceivableReportDto>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
                };

                _loggingService.LogMethodExit($"Returned {result.Items?.Count ?? 0} records out of {result.TotalCount} total");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error searching receivable reports", ex);
                _logger.LogError(ex, "Error searching receivable reports");
                return StatusCode(500, "An error occurred while searching receivable reports");
            }
        }

        /// <summary>
        /// Get all receivable reports
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceivableReportDto>>> GetReceivableReports()
        {
            try
            {
                var reports = await _context.ReceivableReports
                    .Include(rr => rr.Location)
                    .Include(rr => rr.ProcurementMethod!)
                        .ThenInclude(pm => pm.ProcurementType)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ObjectCodeNavigation!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetSubGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.County!)
                    .Where(rr => !rr.IsDeleted)
                    .OrderByDescending(rr => rr.CreatedDate)
                    .ToListAsync();

                var dtos = await MapToDtoListAsync(reports);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving receivable reports");
                return StatusCode(500, "An error occurred while retrieving receivable reports");
            }
        }

        /// <summary>
        /// Get a specific receivable report by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceivableReportDto>> GetReceivableReport(int id)
        {
            _loggingService.LogMethodEntry($"Retrieving receivable report with ID: {id}");
            
            try
            {
                _loggingService.LogInformation($"Fetching receivable report from database - ID: {id}");
                
                var report = await _context.ReceivableReports
                    .Include(rr => rr.Location)
                    .Include(rr => rr.ProcurementMethod!)
                        .ThenInclude(pm => pm.ProcurementType)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ObjectCodeNavigation!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetSubGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.County!)
                    .FirstOrDefaultAsync(rr => rr.Id == id && !rr.IsDeleted);

                if (report == null)
                {
                    _loggingService.LogWarning($"Receivable report not found - ID: {id}");
                    return NotFound($"Receivable report with ID {id} not found");
                }

                _loggingService.LogInformation($"Successfully retrieved receivable report - ID: {id}, Status: {report.RRStatus}");
                var dto = await MapToDtoAsync(report);
                _loggingService.LogMethodExit($"Returning receivable report DTO - ID: {id}");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Error retrieving receivable report with ID: {id}", ex);
                _logger.LogError(ex, "Error retrieving receivable report {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the receivable report");
            }
        }

        /// <summary>
        /// Create a new receivable report
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ReceivableReportDto>> CreateReceivableReport([FromBody] ReceivableReportCreateUpdateDto createDto)
        {
            try
            {
                // Validate FDW entities exist
                var organization = await _fdwContext.GL_Organizations
                    .FirstOrDefaultAsync(o => o.ID == createDto.OrganizationId && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"));
                if (organization == null)
                {
                    return BadRequest($"Organization with ID '{createDto.OrganizationId}' not found or inactive");
                }

                var fund = await _fdwContext.GL_Funds
                    .FirstOrDefaultAsync(f => f.ID == createDto.FundId && (f.EffectiveStatus == "A" || f.EffectiveStatus == "1"));
                if (fund == null)
                {
                    return BadRequest($"Fund with ID '{createDto.FundId}' not found or inactive");
                }

                var oa1 = await _fdwContext.GL_OA1s
                    .FirstOrDefaultAsync(o => o.ID == createDto.OA1Id && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"));
                if (oa1 == null)
                {
                    return BadRequest($"OA1 with ID '{createDto.OA1Id}' not found or inactive");
                }

                // Validate location if provided (by ID or Code)
                if (createDto.LocationId > 0)
                {
                    var locationExists = await _context.Locations
                        .AnyAsync(l => l.Id == createDto.LocationId && l.IsActive);
                    if (!locationExists)
                    {
                        return BadRequest($"Location with ID '{createDto.LocationId}' not found or inactive");
                    }
                }
                else if (!string.IsNullOrEmpty(createDto.LocationCode))
                {
                    var location = await _context.Locations
                        .FirstOrDefaultAsync(l => l.Code == createDto.LocationCode && l.IsActive);
                    if (location == null)
                    {
                        return BadRequest($"Location with code '{createDto.LocationCode}' not found or inactive");
                    }
                    createDto.LocationId = location.Id; // Set the ID for saving
                }

                // NOTE: Assets should NOT have tags when created via this endpoint
                // Asset tags must be generated via the GenerateAndAssignAssetTag endpoint
                // Validate that no assets have tags in the create DTO
                var assetsWithTags = createDto.Assets.Where(a => !string.IsNullOrWhiteSpace(a.AssetTag)).ToList();
                if (assetsWithTags.Any())
                {
                    return BadRequest("Assets cannot be created with tags. Please create assets without tags and use the Generate Tag button to assign unique tags.");
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                // Create the receivable report
                var report = new ReceivableReport
                {
                    OrganizationId = createDto.OrganizationId,
                    FundId = createDto.FundId,
                    OA1Id = createDto.OA1Id,
                    LocationId = createDto.LocationId,
                    LocationCode = createDto.LocationCode,
                    OrderStatus = createDto.OrderStatus,
                    RRStatus = string.IsNullOrEmpty(createDto.RRStatus) ? "Draft" : createDto.RRStatus,
                    AddressLine1 = createDto.AddressLine1,
                    AddressLine2 = createDto.AddressLine2,
                    City = createDto.City,
                    County = createDto.County,
                    State = createDto.State,
                    PostalCode = createDto.PostalCode,
                    CreatedDate = currentTime,
                    CreatedBy = currentUser
                };

                _context.ReceivableReports.Add(report);

                // Create procurement method if provided
                if (createDto.ProcurementMethod != null)
                {
                    // Validate string field lengths to prevent truncation
                    var validationErrors = new List<string>();

                    if (!string.IsNullOrEmpty(createDto.ProcurementMethod.ProcurementTypeCode) && createDto.ProcurementMethod.ProcurementTypeCode.Length > 10)
                        validationErrors.Add($"ProcurementTypeCode is too long (max 10 chars): '{createDto.ProcurementMethod.ProcurementTypeCode}'");

                    if (!string.IsNullOrEmpty(createDto.ProcurementMethod.PcardHolderFirstName) && createDto.ProcurementMethod.PcardHolderFirstName.Length > 50)
                        validationErrors.Add($"PcardHolderFirstName is too long (max 50 chars): '{createDto.ProcurementMethod.PcardHolderFirstName}'");

                    if (!string.IsNullOrEmpty(createDto.ProcurementMethod.PcardHolderLastName) && createDto.ProcurementMethod.PcardHolderLastName.Length > 50)
                        validationErrors.Add($"PcardHolderLastName is too long (max 50 chars): '{createDto.ProcurementMethod.PcardHolderLastName}'");

                    if (!string.IsNullOrEmpty(createDto.ProcurementMethod.PurchaseOrderNumber) && createDto.ProcurementMethod.PurchaseOrderNumber.Length > 50)
                        validationErrors.Add($"PurchaseOrderNumber is too long (max 50 chars): '{createDto.ProcurementMethod.PurchaseOrderNumber}'");

                    if (!string.IsNullOrEmpty(createDto.ProcurementMethod.ContractNumber) && createDto.ProcurementMethod.ContractNumber.Length > 50)
                        validationErrors.Add($"ContractNumber is too long (max 50 chars): '{createDto.ProcurementMethod.ContractNumber}'");

                    if (validationErrors.Any())
                    {
                        return BadRequest($"Validation errors: {string.Join("; ", validationErrors)}");
                    }

                    // Validate ProcurementType exists (by ID or Code)
                    if (createDto.ProcurementMethod.ProcurementTypeId.HasValue)
                    {
                        var procurementTypeExists = await _context.ProcurementTypes
                            .AnyAsync(pt => pt.Id == createDto.ProcurementMethod.ProcurementTypeId.Value && pt.IsActive);
                        
                        if (!procurementTypeExists)
                        {
                            return BadRequest($"Invalid ProcurementTypeId: '{createDto.ProcurementMethod.ProcurementTypeId}'. The specified procurement type does not exist or is inactive.");
                        }
                    }
                    else if (!string.IsNullOrEmpty(createDto.ProcurementMethod.ProcurementTypeCode))
                    {
                        var procurementType = await _context.ProcurementTypes
                            .FirstOrDefaultAsync(pt => pt.Code == createDto.ProcurementMethod.ProcurementTypeCode && pt.IsActive);
                        
                        if (procurementType == null)
                        {
                            return BadRequest($"Invalid ProcurementTypeCode: '{createDto.ProcurementMethod.ProcurementTypeCode}'. The specified procurement type does not exist or is inactive.");
                        }
                        createDto.ProcurementMethod.ProcurementTypeId = procurementType.Id; // Set the ID for saving
                    }

                    var procurementMethod = new ProcurementMethod
                    {
                        ProcurementTypeId = createDto.ProcurementMethod.ProcurementTypeId,
                        ProcurementTypeCode = createDto.ProcurementMethod.ProcurementTypeCode,
                        ChargeDate = createDto.ProcurementMethod.ChargeDate,
                        PcardHolderFirstName = createDto.ProcurementMethod.PcardHolderFirstName,
                        PcardHolderLastName = createDto.ProcurementMethod.PcardHolderLastName,
                        GroupId = createDto.ProcurementMethod.GroupId,
                        PurchaseOrderNumber = createDto.ProcurementMethod.PurchaseOrderNumber,
                        ContractNumber = createDto.ProcurementMethod.ContractNumber,
                        CreatedDate = currentTime,
                        CreatedBy = currentUser
                    };

                    _context.ProcurementMethods.Add(procurementMethod);
                    await _context.SaveChangesAsync();

                    // Link the procurement method to the report
                    report.ProcurementMethodId = procurementMethod.Id;
                    _context.ReceivableReports.Update(report);
                }

                await _context.SaveChangesAsync();

                // NOTE: Assets are now saved individually using the POST {rrId}/asset endpoint
                // This allows assets to be added after the receivable report is created

                // Create history record for INSERT operation
                await CreateHistoryRecord(report, "INSERT", "New receivable report created");

                // Return the created report
                var createdReport = await _context.ReceivableReports
                    .Include(rr => rr.Location)
                    .Include(rr => rr.ProcurementMethod!)
                        .ThenInclude(pm => pm.ProcurementType)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ObjectCodeNavigation!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetSubGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.County!)
                    .FirstAsync(rr => rr.Id == report.Id);

                var dto = await MapToDtoAsync(createdReport);
                return CreatedAtAction(nameof(GetReceivableReport), new { id = report.Id }, dto);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating receivable report");
                
                // Check for specific constraint violations
                if (dbEx.InnerException?.Message.Contains("duplicate key") == true)
                {
                    return BadRequest("Duplicate asset tag detected. Asset tags must be unique.");
                }
                
                return StatusCode(500, $"Database error occurred while creating the receivable report: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating receivable report");
                return StatusCode(500, $"An error occurred while creating the receivable report: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing receivable report
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ReceivableReportDto>> UpdateReceivableReport(int id, [FromBody] ReceivableReportCreateUpdateDto updateDto)
        {
            try
            {
                // Validate the DTO
                if (updateDto == null)
                {
                    return BadRequest("Update data is required");
                }

                if (updateDto.OrganizationId <= 0)
                {
                    return BadRequest("OrganizationId is required");
                }

                if (updateDto.FundId <= 0)
                {
                    return BadRequest("FundId is required");
                }

                if (updateDto.OA1Id <= 0)
                {
                    return BadRequest("OA1Id is required");
                }

                if (string.IsNullOrEmpty(updateDto.OrderStatus))
                {
                    return BadRequest("Order Status is required");
                }

                var report = await _context.ReceivableReports
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                    .FirstOrDefaultAsync(rr => rr.Id == id && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable report with ID {id} not found");
                }

                // Validate FDW entities exist
                var organization = await _fdwContext.GL_Organizations
                    .FirstOrDefaultAsync(o => o.ID == updateDto.OrganizationId && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"));
                if (organization == null)
                {
                    return BadRequest($"Organization with ID '{updateDto.OrganizationId}' not found or inactive");
                }

                var fund = await _fdwContext.GL_Funds
                    .FirstOrDefaultAsync(f => f.ID == updateDto.FundId && (f.EffectiveStatus == "A" || f.EffectiveStatus == "1"));
                if (fund == null)
                {
                    return BadRequest($"Fund with ID '{updateDto.FundId}' not found or inactive");
                }

                var oa1 = await _fdwContext.GL_OA1s
                    .FirstOrDefaultAsync(o => o.ID == updateDto.OA1Id && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"));
                if (oa1 == null)
                {
                    return BadRequest($"OA1 with ID '{updateDto.OA1Id}' not found or inactive");
                }

                // Validate location if provided
                if (!string.IsNullOrEmpty(updateDto.LocationCode))
                {
                    var locationExists = await _context.Locations
                        .AnyAsync(l => l.Code == updateDto.LocationCode && l.IsActive);
                    if (!locationExists)
                    {
                        return BadRequest($"Location with code '{updateDto.LocationCode}' not found or inactive");
                    }
                }

                // NOTE: Assets should NOT have tags updated via this endpoint
                // Asset tags must be generated/updated via the GenerateAndAssignAssetTag or UpdateAssetTag endpoints
                // Validate that no assets have tags in the update DTO
                var assetsWithTags = updateDto.Assets.Where(a => !string.IsNullOrWhiteSpace(a.AssetTag)).ToList();
                if (assetsWithTags.Any())
                {
                    return BadRequest("Assets cannot be updated with tags via this endpoint. Please use the Generate Tag button or Update Asset Tag endpoint to assign unique tags.");
                }

                // Validate object code IDs exist
                var objectCodeIds = updateDto.Assets.Where(a => a.ObjectCodeId.HasValue).Select(a => a.ObjectCodeId!.Value).Distinct().ToList();
                if (objectCodeIds.Any())
                {
                    var invalidObjectCodeIds = new List<int>();
                    foreach (var objectCodeId in objectCodeIds)
                    {
                        var exists = await _context.ObjectCodes.AnyAsync(oc => oc.Id == objectCodeId && oc.IsActive);
                        if (!exists)
                        {
                            invalidObjectCodeIds.Add(objectCodeId);
                        }
                    }
                    
                    if (invalidObjectCodeIds.Any())
                    {
                        return BadRequest($"Invalid object code IDs: {string.Join(", ", invalidObjectCodeIds)}");
                    }
                }

                // Validate county IDs exist
                var countyIds = updateDto.Assets.Where(a => a.CountyId.HasValue).Select(a => a.CountyId!.Value).Distinct().ToList();
                if (countyIds.Any())
                {
                    var invalidCountyIds = new List<int>();
                    foreach (var countyId in countyIds)
                    {
                        var exists = await _context.Counties.AnyAsync(c => c.Id == countyId && c.IsActive);
                        if (!exists)
                        {
                            invalidCountyIds.Add(countyId);
                        }
                    }
                    
                    if (invalidCountyIds.Any())
                    {
                        return BadRequest($"Invalid county IDs: {string.Join(", ", invalidCountyIds)}");
                    }
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                // Update report properties
                report.OrganizationId = updateDto.OrganizationId;
                report.FundId = updateDto.FundId;
                report.OA1Id = updateDto.OA1Id;
                report.LocationCode = updateDto.LocationCode;
                report.OrderStatus = updateDto.OrderStatus;
                if (!string.IsNullOrEmpty(updateDto.RRStatus))
                {
                    report.RRStatus = updateDto.RRStatus;
                }
                report.AddressLine1 = updateDto.AddressLine1;
                report.AddressLine2 = updateDto.AddressLine2;
                report.City = updateDto.City;
                report.County = updateDto.County;
                report.State = updateDto.State;
                report.PostalCode = updateDto.PostalCode;
                report.ModifiedDate = currentTime;
                report.ModifiedBy = currentUser;

                // Handle procurement method updates
                if (updateDto.ProcurementMethod != null)
                {
                    // Validate string field lengths to prevent truncation
                    var validationErrors = new List<string>();

                    if (!string.IsNullOrEmpty(updateDto.ProcurementMethod.ProcurementTypeCode) && updateDto.ProcurementMethod.ProcurementTypeCode.Length > 10)
                        validationErrors.Add($"ProcurementTypeCode is too long (max 10 chars): '{updateDto.ProcurementMethod.ProcurementTypeCode}'");

                    if (!string.IsNullOrEmpty(updateDto.ProcurementMethod.PcardHolderFirstName) && updateDto.ProcurementMethod.PcardHolderFirstName.Length > 50)
                        validationErrors.Add($"PcardHolderFirstName is too long (max 50 chars): '{updateDto.ProcurementMethod.PcardHolderFirstName}'");

                    if (!string.IsNullOrEmpty(updateDto.ProcurementMethod.PcardHolderLastName) && updateDto.ProcurementMethod.PcardHolderLastName.Length > 50)
                        validationErrors.Add($"PcardHolderLastName is too long (max 50 chars): '{updateDto.ProcurementMethod.PcardHolderLastName}'");

                    if (!string.IsNullOrEmpty(updateDto.ProcurementMethod.PurchaseOrderNumber) && updateDto.ProcurementMethod.PurchaseOrderNumber.Length > 50)
                        validationErrors.Add($"PurchaseOrderNumber is too long (max 50 chars): '{updateDto.ProcurementMethod.PurchaseOrderNumber}'");

                    if (!string.IsNullOrEmpty(updateDto.ProcurementMethod.ContractNumber) && updateDto.ProcurementMethod.ContractNumber.Length > 50)
                        validationErrors.Add($"ContractNumber is too long (max 50 chars): '{updateDto.ProcurementMethod.ContractNumber}'");

                    if (validationErrors.Any())
                    {
                        return BadRequest($"Validation errors: {string.Join("; ", validationErrors)}");
                    }

                    // Validate ProcurementTypeCode exists
                    if (!string.IsNullOrEmpty(updateDto.ProcurementMethod.ProcurementTypeCode))
                    {
                        var procurementTypeExists = await _context.ProcurementTypes
                            .AnyAsync(pt => pt.Code == updateDto.ProcurementMethod.ProcurementTypeCode && pt.IsActive);
                        
                        if (!procurementTypeExists)
                        {
                            return BadRequest($"Invalid ProcurementTypeCode: '{updateDto.ProcurementMethod.ProcurementTypeCode}'. The specified procurement type does not exist or is inactive.");
                        }
                    }

                    if (report.ProcurementMethodId.HasValue)
                    {
                        // Update existing procurement method
                        var existingProcurementMethod = await _context.ProcurementMethods
                            .FirstOrDefaultAsync(pm => pm.Id == report.ProcurementMethodId.Value);
                        
                        if (existingProcurementMethod != null)
                        {
                            existingProcurementMethod.ProcurementTypeCode = updateDto.ProcurementMethod.ProcurementTypeCode;
                            existingProcurementMethod.ChargeDate = updateDto.ProcurementMethod.ChargeDate;
                            existingProcurementMethod.PcardHolderFirstName = updateDto.ProcurementMethod.PcardHolderFirstName;
                            existingProcurementMethod.PcardHolderLastName = updateDto.ProcurementMethod.PcardHolderLastName;
                            existingProcurementMethod.GroupId = updateDto.ProcurementMethod.GroupId;
                            existingProcurementMethod.PurchaseOrderNumber = updateDto.ProcurementMethod.PurchaseOrderNumber;
                            existingProcurementMethod.ContractNumber = updateDto.ProcurementMethod.ContractNumber;
                            existingProcurementMethod.ModifiedDate = currentTime;
                            existingProcurementMethod.ModifiedBy = currentUser;
                        }
                    }
                    else
                    {
                        // Create new procurement method
                        var newProcurementMethod = new ProcurementMethod
                        {
                            ProcurementTypeCode = updateDto.ProcurementMethod.ProcurementTypeCode,
                            ChargeDate = updateDto.ProcurementMethod.ChargeDate,
                            PcardHolderFirstName = updateDto.ProcurementMethod.PcardHolderFirstName,
                            PcardHolderLastName = updateDto.ProcurementMethod.PcardHolderLastName,
                            GroupId = updateDto.ProcurementMethod.GroupId,
                            PurchaseOrderNumber = updateDto.ProcurementMethod.PurchaseOrderNumber,
                            ContractNumber = updateDto.ProcurementMethod.ContractNumber,
                            CreatedDate = currentTime,
                            CreatedBy = currentUser
                        };

                        _context.ProcurementMethods.Add(newProcurementMethod);
                        await _context.SaveChangesAsync();
                        report.ProcurementMethodId = newProcurementMethod.Id;
                    }
                }
                else
                {
                    // Remove procurement method reference if null
                    report.ProcurementMethodId = null;
                }

                // Handle asset updates
                var existingAssetIds = report.Assets.Select(a => a.Id).ToList();
                var updatedAssetIds = updateDto.Assets.Where(a => a.Id > 0).Select(a => a.Id).ToList();

                // Mark assets for deletion that are not in the update
                var assetsToDelete = report.Assets.Where(a => !updatedAssetIds.Contains(a.Id)).ToList();
                foreach (var asset in assetsToDelete)
                {
                    asset.IsDeleted = true;
                    asset.DeletedDate = currentTime;
                    asset.DeletedBy = currentUser;
                }

                // NOTE: Assets are now saved individually using the POST {rrId}/asset endpoint
                // This allows for real-time asset saving and grid refresh

                await _context.SaveChangesAsync();

                // Create history record for UPDATE operation
                await CreateHistoryRecord(report, "UPDATE", "Receivable report updated");

                // Return updated report
                var updatedReport = await _context.ReceivableReports
                    .Include(rr => rr.Location)
                    .Include(rr => rr.ProcurementMethod!)
                        .ThenInclude(pm => pm.ProcurementType)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ObjectCodeNavigation!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.AssetSubGroup!)
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.County!)
                    .FirstAsync(rr => rr.Id == id);

                var dto = await MapToDtoAsync(updatedReport);
                return Ok(dto);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error updating receivable report {Id}", id);
                
                // Check for specific constraint violations
                if (dbEx.InnerException?.Message.Contains("duplicate key") == true)
                {
                    return BadRequest("Duplicate asset tag detected. Asset tags must be unique.");
                }
                
                return StatusCode(500, $"Database error occurred while updating the receivable report: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating receivable report {Id}", id);
                return StatusCode(500, $"An error occurred while updating the receivable report: {ex.Message}");
            }
        }

        /// <summary>
        /// Save a single asset (create new or update existing)
        /// </summary>
        [HttpPost("{rrId}/asset")]
        public async Task<ActionResult<AssetDto>> SaveAsset(int rrId, [FromBody] AssetCreateUpdateDto assetDto)
        {
            try
            {
                // Verify receivable report exists
                var report = await _context.ReceivableReports
                    .FirstOrDefaultAsync(rr => rr.Id == rrId && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable report with ID {rrId} not found");
                }

                // Validate ObjectCodeId (REQUIRED)
                if (!assetDto.ObjectCodeId.HasValue || assetDto.ObjectCodeId.Value <= 0)
                {
                    return BadRequest("Object Code is required");
                }

                // Prevent AssetTag from being set via SaveAsset - it should only be set via GenerateAndAssignAssetTag endpoint
                if (!string.IsNullOrWhiteSpace(assetDto.AssetTag))
                {
                    return BadRequest("AssetTag cannot be set via SaveAsset endpoint. Please use the Generate Tag button to assign a unique tag.");
                }

                var objectCodeExists = await _context.ObjectCodes
                    .AnyAsync(oc => oc.Id == assetDto.ObjectCodeId.Value && oc.IsActive);
                if (!objectCodeExists)
                {
                    return BadRequest($"Invalid object code ID: {assetDto.ObjectCodeId.Value}. The selected object code does not exist or is inactive.");
                }

                // Validate CountyId (REQUIRED only if IsOwnedByCounty is true)
                if (assetDto.IsOwnedByCounty)
                {
                    if (!assetDto.CountyId.HasValue || assetDto.CountyId.Value <= 0)
                    {
                        return BadRequest("County is required when 'Owned by County' is selected");
                    }

                    var countyExists = await _context.Counties
                        .AnyAsync(c => c.Id == assetDto.CountyId.Value && c.IsActive);
                    if (!countyExists)
                    {
                        return BadRequest($"Invalid county ID: {assetDto.CountyId.Value}. The selected county does not exist or is inactive.");
                    }
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                Asset savedAsset;

                if (assetDto.Id > 0)
                {
                    // Update existing asset
                    var existingAsset = await _context.Assets
                        .FirstOrDefaultAsync(a => a.Id == assetDto.Id && a.ReceivableReportId == rrId && !a.IsDeleted);

                    if (existingAsset == null)
                    {
                        return NotFound($"Asset with ID {assetDto.Id} not found in this receivable report");
                    }

                    existingAsset.Brand = assetDto.Brand;
                    existingAsset.Make = assetDto.Make;
                    existingAsset.Model = assetDto.Model;
                    // AssetTag is NOT updated via SaveAsset - it can only be set via GenerateAndAssignAssetTag endpoint
                    // This prevents accidental duplicate tags
                    existingAsset.SerialNumber = assetDto.SerialNumber;
                    existingAsset.AssetValue = assetDto.AssetValue;
                    existingAsset.ObjectCodeId = assetDto.ObjectCodeId;
                    existingAsset.AssetGroupId = assetDto.AssetGroupId;
                    existingAsset.AssetSubGroupId = assetDto.AssetSubGroupId;
                    existingAsset.AssignedTo = assetDto.AssignedTo;
                    existingAsset.Floor = assetDto.Floor;
                    existingAsset.Room = assetDto.Room;
                    existingAsset.IsOwnedByCounty = assetDto.IsOwnedByCounty;
                    existingAsset.CountyId = assetDto.CountyId;
                    existingAsset.ModifiedDate = currentTime;
                    existingAsset.ModifiedBy = currentUser;

                    savedAsset = existingAsset;
                }
                else
                {
                    // Create new asset
                    // NOTE: AssetTag is NOT set here - it must be generated via GenerateAndAssignAssetTag endpoint
                    // This prevents duplicate tags and ensures uniqueness
                    var newAsset = new Asset
                    {
                        ReceivableReportId = rrId,
                        Brand = assetDto.Brand,
                        Make = assetDto.Make,
                        Model = assetDto.Model,
                        AssetTag = null, // Always null - must be generated via dedicated endpoint
                        SerialNumber = assetDto.SerialNumber,
                        AssetValue = assetDto.AssetValue,
                        ObjectCodeId = assetDto.ObjectCodeId,
                        AssetGroupId = assetDto.AssetGroupId,
                        AssetSubGroupId = assetDto.AssetSubGroupId,
                        AssignedTo = assetDto.AssignedTo,
                        Floor = assetDto.Floor,
                        Room = assetDto.Room,
                        IsOwnedByCounty = assetDto.IsOwnedByCounty,
                        CountyId = assetDto.CountyId,
                        UniqueTagNumber = assetDto.UniqueTagNumber,
                        AssetStatus = assetDto.AssetStatus,
                        CreatedDate = currentTime,
                        CreatedBy = currentUser
                    };

                    _context.Assets.Add(newAsset);
                    savedAsset = newAsset;
                }

                await _context.SaveChangesAsync();

                // Reload asset with navigation properties
                var assetWithNav = await _context.Assets
                    .Include(a => a.ObjectCodeNavigation)
                    .Include(a => a.AssetGroup)
                    .Include(a => a.AssetSubGroup)
                    .Include(a => a.County)
                    .FirstOrDefaultAsync(a => a.Id == savedAsset.Id);

                return Ok(MapAssetToDto(assetWithNav!));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error saving asset for receivable report {RRId}", rrId);
                
                if (dbEx.InnerException?.Message.Contains("IX_Assets_AssetTag") == true)
                {
                    return BadRequest("Duplicate asset tag detected. Asset tags must be unique.");
                }
                
                return StatusCode(500, $"Database error occurred while saving the asset: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving asset for receivable report {RRId}", rrId);
                return StatusCode(500, $"An error occurred while saving the asset: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all assets for a receivable report
        /// </summary>
        [HttpGet("{rrId}/assets")]
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssetsByReceivableReport(int rrId)
        {
            try
            {
                var assets = await _context.Assets
                    .Include(a => a.ObjectCodeNavigation)
                    .Include(a => a.County)
                    .Where(a => a.ReceivableReportId == rrId && !a.IsDeleted)
                    .OrderBy(a => a.CreatedDate)
                    .ToListAsync();

                return Ok(assets.Select(MapAssetToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assets for receivable report {RRId}", rrId);
                return StatusCode(500, $"An error occurred while retrieving assets: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete an asset
        /// </summary>
        [HttpDelete("{rrId}/asset/{assetId}")]
        public async Task<ActionResult> DeleteAsset(int rrId, int assetId)
        {
            try
            {
                var asset = await _context.Assets
                    .FirstOrDefaultAsync(a => a.Id == assetId && a.ReceivableReportId == rrId && !a.IsDeleted);

                if (asset == null)
                {
                    return NotFound($"Asset with ID {assetId} not found in receivable report {rrId}");
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                asset.IsDeleted = true;
                asset.DeletedDate = currentTime;
                asset.DeletedBy = currentUser;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset {AssetId} from receivable report {RRId}", assetId, rrId);
                return StatusCode(500, $"An error occurred while deleting the asset: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate and assign a unique asset tag to a specific asset (atomic operation)
        /// This endpoint combines generation and assignment to prevent race conditions
        /// </summary>
        [HttpPost("{rrId}/asset/{assetId}/generate-asset-tag")]
        public async Task<ActionResult<AssetDto>> GenerateAndAssignAssetTag(int rrId, int assetId)
        {
            // Log which database we're connecting to
            var connectionString = _context.Database.GetConnectionString();
            var databaseName = _context.Database.GetDbConnection().Database;
            _logger.LogInformation("Generating asset tag for asset {AssetId} in RR {RRId}. Database: {DatabaseName}, Connection: {ConnectionString}", 
                assetId, rrId, databaseName, connectionString?.Substring(0, Math.Min(100, connectionString?.Length ?? 0)));
            
            const int maxRetries = 5;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    // Use a transaction with SERIALIZABLE isolation level to prevent race conditions
                    // SERIALIZABLE is the highest isolation level and prevents phantom reads
                    using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                    try
                    {
                        // Note: BeginTransactionAsync automatically makes EF Core use this transaction
                        // No need to call UseTransactionAsync - it's already active
                        
                        // Verify which database we're actually using
                        var actualDbName = _context.Database.GetDbConnection().Database;
                        _logger.LogInformation("Transaction started. Actual database: {DatabaseName}", actualDbName);
                        
                        // Safety check: Warn if we're using production database in development
                        if (actualDbName == "AssetManagerDb" && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                        {
                            _logger.LogWarning("WARNING: Using PRODUCTION database (AssetManagerDb) in Development environment! This may cause duplicate tag errors.");
                        }
                        
                        // Verify receivable report exists
                        var report = await _context.ReceivableReports
                            .FirstOrDefaultAsync(rr => rr.Id == rrId && !rr.IsDeleted);

                        if (report == null)
                        {
                            await transaction.RollbackAsync();
                            return NotFound($"Receivable report with ID {rrId} not found");
                        }

                        // Verify asset exists and belongs to this report
                        // Load within transaction to ensure we see the latest state
                        var asset = await _context.Assets
                            .FirstOrDefaultAsync(a => a.Id == assetId && a.ReceivableReportId == rrId && !a.IsDeleted);

                        if (asset == null)
                        {
                            await transaction.RollbackAsync();
                            return NotFound($"Asset with ID {assetId} not found in receivable report {rrId}");
                        }

                        // Check if asset already has a tag (double-check within transaction)
                        if (!string.IsNullOrWhiteSpace(asset.AssetTag))
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"Asset already has a tag: {asset.AssetTag}. Use reset to clear it first.");
                        }

                        // Get organization info from FDW database
                        var organization = await _fdwContext.GL_Organizations
                            .FirstOrDefaultAsync(o => o.ID == report.OrganizationId);

                        var orgCode = organization?.Organization?.Substring(0, Math.Min(3, organization.Organization.Length)).ToUpper() ?? "AST";
                        var year = DateTime.UtcNow.Year.ToString().Substring(2, 2); // Last 2 digits of year
                        
                        // Use SERIALIZABLE isolation level with TABLOCKX to lock entire table
                        // This ensures only one transaction can read and generate at a time
                        var pattern = $"{orgCode}-{year}-%";
                        
                        // Get the connection and transaction
                        var connection = _context.Database.GetDbConnection();
                        var dbTransaction = transaction.GetDbTransaction();
                        
                        if (connection.State != System.Data.ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }
                        
                        // Verify which database we're actually querying
                        string? actualDbInQuery = null;
                        using (var dbCheckCommand = connection.CreateCommand())
                        {
                            dbCheckCommand.Transaction = dbTransaction;
                            dbCheckCommand.CommandText = "SELECT DB_NAME()";
                            var dbResult = await dbCheckCommand.ExecuteScalarAsync();
                            actualDbInQuery = dbResult?.ToString();
                            _logger.LogInformation("SQL Query executing against database: {DatabaseName}", actualDbInQuery);
                        }
                        
                        // Lock the entire Assets table to prevent concurrent tag generation
                        // Use TABLOCKX for exclusive table lock during tag generation
                        // IMPORTANT: Check ALL assets (including deleted) for tag uniqueness
                        // The unique constraint applies to all assets regardless of deletion status
                        string? lastTag = null;
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = dbTransaction;
                            command.CommandText = @"
                                SELECT MAX(AssetTag) 
                                FROM Assets WITH (TABLOCKX)
                                WHERE AssetTag IS NOT NULL
                                AND AssetTag LIKE @pattern";
                            
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = "@pattern";
                            parameter.Value = pattern;
                            parameter.DbType = System.Data.DbType.String;
                            command.Parameters.Add(parameter);
                            
                            var result = await command.ExecuteScalarAsync();
                            lastTag = result?.ToString();
                            _logger.LogInformation("Found last tag: {LastTag} in database: {DatabaseName} (checked all assets including deleted)", lastTag, actualDbInQuery);
                        }
                        
                        int nextNumber = 1;
                        if (!string.IsNullOrEmpty(lastTag))
                        {
                            var parts = lastTag.Split('-');
                            if (parts.Length >= 3 && int.TryParse(parts[2], out int lastNumber))
                            {
                                nextNumber = lastNumber + 1;
                            }
                        }

                        var generatedTag = $"{orgCode}-{year}-{nextNumber:D4}"; // e.g., "FIN-25-0001"

                        // Verify the tag doesn't exist (double-check with table lock still held)
                        // Check ALL assets (including deleted) because unique constraint applies to all
                        int tagExistsCount = 0;
                        using (var checkCommand = connection.CreateCommand())
                        {
                            checkCommand.Transaction = dbTransaction;
                            checkCommand.CommandText = @"
                                SELECT COUNT(*) 
                                FROM Assets WITH (TABLOCKX)
                                WHERE AssetTag = @tag";
                            
                            var checkParameter = checkCommand.CreateParameter();
                            checkParameter.ParameterName = "@tag";
                            checkParameter.Value = generatedTag;
                            checkParameter.DbType = System.Data.DbType.String;
                            checkCommand.Parameters.Add(checkParameter);
                            
                            var tagExistsResult = await checkCommand.ExecuteScalarAsync();
                            tagExistsCount = tagExistsResult != null ? Convert.ToInt32(tagExistsResult) : 0;
                            _logger.LogInformation("Tag existence check for '{GeneratedTag}': Found {Count} assets (including deleted)", generatedTag, tagExistsCount);
                        }

                        if (tagExistsCount > 0)
                        {
                            // Tag was created between our query and now, retry
                            await transaction.RollbackAsync();
                            retryCount++;
                            await Task.Delay(100 * retryCount); // Exponential backoff
                            continue;
                        }

                        // Final check: Verify asset still doesn't have a tag (in case it was set by another process)
                        // Reload asset within transaction to ensure we see the latest state
                        await _context.Entry(asset).ReloadAsync();
                        
                        if (!string.IsNullOrWhiteSpace(asset.AssetTag))
                        {
                            await transaction.RollbackAsync();
                            retryCount++;
                            await Task.Delay(100 * retryCount);
                            continue;
                        }

                        // Final validation: Check tag one more time right before assignment
                        // This is the last chance to catch any race condition
                        // Check ALL assets (including deleted) because unique constraint applies to all
                        int finalCheckCount = 0;
                        using (var finalCheckCommand = connection.CreateCommand())
                        {
                            finalCheckCommand.Transaction = dbTransaction;
                            finalCheckCommand.CommandText = @"
                                SELECT COUNT(*) 
                                FROM Assets WITH (TABLOCKX)
                                WHERE AssetTag = @tag";
                            
                            var finalCheckParameter = finalCheckCommand.CreateParameter();
                            finalCheckParameter.ParameterName = "@tag";
                            finalCheckParameter.Value = generatedTag;
                            finalCheckParameter.DbType = System.Data.DbType.String;
                            finalCheckCommand.Parameters.Add(finalCheckParameter);
                            
                            var finalCheckResult = await finalCheckCommand.ExecuteScalarAsync();
                            finalCheckCount = finalCheckResult != null ? Convert.ToInt32(finalCheckResult) : 0;
                            _logger.LogInformation("Final tag validation for '{GeneratedTag}': Found {Count} assets (including deleted)", generatedTag, finalCheckCount);
                        }

                        if (finalCheckCount > 0)
                        {
                            // Tag was created at the last moment, retry
                            await transaction.RollbackAsync();
                            retryCount++;
                            await Task.Delay(100 * retryCount);
                            continue;
                        }

                        // Assign the tag to the asset
                        var currentUser = GetCurrentUser();
                        var currentTime = DateTime.UtcNow;

                        asset.AssetTag = generatedTag;
                        asset.ModifiedDate = currentTime;
                        asset.ModifiedBy = currentUser;

                        // SaveChanges will use the transaction we set with UseTransactionAsync
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        // Reload asset with navigation properties
                        var assetWithNav = await _context.Assets
                            .Include(a => a.ObjectCodeNavigation)
                            .Include(a => a.AssetGroup)
                            .Include(a => a.AssetSubGroup)
                            .Include(a => a.County)
                            .FirstOrDefaultAsync(a => a.Id == assetId);

                        _logger.LogInformation("Generated and assigned asset tag {AssetTag} to asset {AssetId} in receivable report {RRId}", generatedTag, assetId, rrId);

                        return Ok(MapAssetToDto(assetWithNav!));
                    }
                    catch (Exception innerEx)
                    {
                        try
                        {
                            await transaction.RollbackAsync();
                        }
                        catch (Exception rollbackEx)
                        {
                            _logger.LogError(rollbackEx, "Error rolling back transaction for asset {AssetId}", assetId);
                        }
                        _logger.LogError(innerEx, "Inner exception in transaction for asset {AssetId}: {Message}, Type: {Type}", assetId, innerEx.Message, innerEx.GetType().Name);
                        throw; // Re-throw to be caught by outer catch
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    // Check if it's a duplicate key error (shouldn't happen with our locking, but handle it)
                    if (dbEx.InnerException?.Message.Contains("IX_Assets_AssetTag") == true)
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            _logger.LogError(dbEx, "Failed to generate unique asset tag after {RetryCount} retries for asset {AssetId}", retryCount, assetId);
                            return StatusCode(500, "Unable to generate unique asset tag after multiple attempts. Please try again.");
                        }
                        await Task.Delay(100 * retryCount);
                        continue;
                    }
                    _logger.LogError(dbEx, "Database error generating and assigning asset tag for asset {AssetId} in receivable report {RRId}", assetId, rrId);
                    return StatusCode(500, $"Database error while generating asset tag: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating and assigning asset tag for asset {AssetId} in receivable report {RRId}. Exception: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}", 
                        assetId, rrId, ex.GetType().Name, ex.Message, ex.StackTrace);
                    return StatusCode(500, $"An error occurred while generating the asset tag for asset {assetId}: {ex.Message}");
                }
            }

            return StatusCode(500, "Unable to generate unique asset tag after multiple attempts. Please try again.");
        }

        /// <summary>
        /// Generate a unique asset tag for a receivable report (legacy endpoint - kept for backward compatibility)
        /// NOTE: This endpoint only generates a tag but doesn't assign it. Use GenerateAndAssignAssetTag for atomic operation.
        /// </summary>
        [HttpPost("{rrId}/generate-asset-tag")]
        public async Task<ActionResult<object>> GenerateAssetTag(int rrId)
        {
            try
            {
                // Verify receivable report exists
                var report = await _context.ReceivableReports
                    .FirstOrDefaultAsync(rr => rr.Id == rrId && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable report with ID {rrId} not found");
                }

                // Get organization info from FDW database
                var organization = await _fdwContext.GL_Organizations
                    .FirstOrDefaultAsync(o => o.ID == report.OrganizationId);

                var orgCode = organization?.Organization?.Substring(0, Math.Min(3, organization.Organization.Length)).ToUpper() ?? "AST";
                var year = DateTime.UtcNow.Year.ToString().Substring(2, 2); // Last 2 digits of year
                
                // Find highest sequential number for this organization-year pattern
                var pattern = $"{orgCode}-{year}-%";
                var lastTag = await _context.Assets
                    .Where(a => !a.IsDeleted && EF.Functions.Like(a.AssetTag, pattern))
                    .OrderByDescending(a => a.AssetTag)
                    .Select(a => a.AssetTag)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;
                if (!string.IsNullOrEmpty(lastTag))
                {
                    var parts = lastTag.Split('-');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                var generatedTag = $"{orgCode}-{year}-{nextNumber:D4}"; // e.g., "FIN-25-0001"

                _logger.LogInformation("Generated asset tag {AssetTag} for receivable report {RRId}", generatedTag, rrId);

                return Ok(new { assetTag = generatedTag });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating asset tag for receivable report {RRId}", rrId);
                return StatusCode(500, $"An error occurred while generating the asset tag: {ex.Message}");
            }
        }

        /// <summary>
        /// Update asset tag for a specific asset
        /// </summary>
        [HttpPatch("{rrId}/asset/{assetId}/asset-tag")]
        public async Task<ActionResult<AssetDto>> UpdateAssetTag(int rrId, int assetId, [FromBody] UpdateAssetTagDto updateDto)
        {
            try
            {
                var asset = await _context.Assets
                    .FirstOrDefaultAsync(a => a.Id == assetId && a.ReceivableReportId == rrId && !a.IsDeleted);

                if (asset == null)
                {
                    return NotFound($"Asset with ID {assetId} not found in receivable report {rrId}");
                }

                // Check if new asset tag already exists (excluding current asset)
                // Skip check if AssetTag is null (resetting the tag)
                if (!string.IsNullOrWhiteSpace(updateDto.AssetTag))
                {
                    var tagExists = await _context.Assets
                        .AnyAsync(a => a.AssetTag == updateDto.AssetTag && a.Id != assetId && !a.IsDeleted);

                    if (tagExists)
                    {
                        return BadRequest($"Asset tag '{updateDto.AssetTag}' already exists. Please use a unique tag.");
                    }
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                asset.AssetTag = updateDto.AssetTag;
                asset.ModifiedDate = currentTime;
                asset.ModifiedBy = currentUser;

                // If resetting tag (setting to NULL), also reset status to "Open"
                if (string.IsNullOrWhiteSpace(updateDto.AssetTag))
                {
                    asset.AssetStatus = "Open";
                    _logger.LogInformation("Asset {AssetId} status reset to 'Open' due to tag reset", assetId);
                }

                await _context.SaveChangesAsync();

                // Reload asset with navigation properties
                var assetWithNav = await _context.Assets
                    .Include(a => a.ObjectCodeNavigation)
                    .Include(a => a.AssetGroup)
                    .Include(a => a.AssetSubGroup)
                    .Include(a => a.County)
                    .FirstOrDefaultAsync(a => a.Id == assetId);

                var actionDescription = string.IsNullOrWhiteSpace(updateDto.AssetTag) ? "reset (cleared)" : $"updated to '{updateDto.AssetTag}'";
                _logger.LogInformation("Asset tag for asset {AssetId} {ActionDescription}", assetId, actionDescription);

                return Ok(MapAssetToDto(assetWithNav!));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error updating asset tag for asset {AssetId}", assetId);
                
                if (dbEx.InnerException?.Message.Contains("IX_Assets_AssetTag") == true)
                {
                    return BadRequest("Duplicate asset tag detected. Asset tags must be unique.");
                }
                
                return StatusCode(500, $"Database error occurred while updating the asset tag: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset tag for asset {AssetId}", assetId);
                return StatusCode(500, $"An error occurred while updating the asset tag: {ex.Message}");
            }
        }

        /// <summary>
        /// Get available procurement types for dropdown
        /// </summary>
        [HttpGet("procurement-types")]
        public async Task<ActionResult<IEnumerable<object>>> GetProcurementTypes()
        {
            try
            {
                var procurementTypes = await _context.ProcurementTypes
                    .Where(pt => pt.IsActive)
                    .OrderBy(pt => pt.Name)
                    .Select(pt => new { 
                        Code = pt.Code, 
                        Name = pt.Name,
                        Description = pt.Description 
                    })
                    .ToListAsync();

                return Ok(procurementTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving procurement types");
                return StatusCode(500, "An error occurred while retrieving procurement types");
            }
        }

        /// <summary>
        /// Delete a receivable report (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceivableReport(int id)
        {
            try
            {
                var report = await _context.ReceivableReports
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                    .FirstOrDefaultAsync(rr => rr.Id == id && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable report with ID {id} not found");
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                // Soft delete the report
                report.IsDeleted = true;
                report.DeletedDate = currentTime;
                report.DeletedBy = currentUser;

                // Soft delete all associated assets
                foreach (var asset in report.Assets)
                {
                    asset.IsDeleted = true;
                    asset.DeletedDate = currentTime;
                    asset.DeletedBy = currentUser;
                }

                await _context.SaveChangesAsync();

                // Create history record for DELETE operation
                await CreateHistoryRecord(report, "DELETE", "Receivable report deleted");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting receivable report {Id}", id);
                return StatusCode(500, "An error occurred while deleting the receivable report");
            }
        }

        /// <summary>
        /// Attest tags for a receivable report
        /// </summary>
        [HttpPost("{id}/attest-tags")]
        public async Task<IActionResult> AttestTags(int id)
        {
            try
            {
                var report = await _context.ReceivableReports
                    .FirstOrDefaultAsync(rr => rr.Id == id && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable report with ID {id} not found");
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                report.AttestedDate = currentTime;
                report.AttestedBy = currentUser;
                report.ModifiedDate = currentTime;
                report.ModifiedBy = currentUser;

                await _context.SaveChangesAsync();

                // Create history record for ATTEST operation
                await CreateHistoryRecord(report, "UPDATE", "Tags attested");

                return Ok(new { Message = "Tags attested successfully", AttestedDate = currentTime, AttestedBy = currentUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attesting tags for receivable report {Id}", id);
                return StatusCode(500, "An error occurred while attesting tags");
            }
        }

        /// <summary>
        /// Get history records for a receivable report
        /// </summary>
        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<ReceivableReportHistory>>> GetReceivableReportHistory(int id)
        {
            try
            {
                var historyRecords = await _context.ReceivableReportHistory
                    .Where(h => h.ReceivableReportId == id)
                    .OrderByDescending(h => h.HistoryDate)
                    .ToListAsync();

                return Ok(historyRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for receivable report {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the receivable report history");
            }
        }

        /// <summary>
        /// Print tag for a specific asset
        /// </summary>
        [HttpPost("asset/{assetId}/print-tag")]
        public async Task<IActionResult> PrintAssetTag(int assetId)
        {
            try
            {
                var asset = await _context.Assets
                    .Include(a => a.ReceivableReport)
                    .FirstOrDefaultAsync(a => a.Id == assetId && !a.IsDeleted);

                if (asset == null)
                {
                    return NotFound($"Asset with ID {assetId} not found");
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                // Generate unique tag number if not already generated
                if (string.IsNullOrEmpty(asset.UniqueTagNumber))
                {
                    // Get organization code from FDW database
                    var orgForTag = await _fdwContext.GL_Organizations
                        .FirstOrDefaultAsync(o => o.ID == asset.ReceivableReport.OrganizationId);
                    var orgCode = orgForTag?.Code ?? "UNK";
                    asset.UniqueTagNumber = GenerateUniqueTagNumber(orgCode);
                }

                // Update asset status to Printed Tag
                asset.AssetStatus = "Printed Tag";
                asset.TagPrintedDate = currentTime;
                asset.TagPrintedBy = currentUser;
                asset.ModifiedDate = currentTime;
                asset.ModifiedBy = currentUser;

                await _context.SaveChangesAsync();

                // Generate print data for Avery label
                // Get organization data from FDW database
                var organization = await _fdwContext.GL_Organizations
                    .FirstOrDefaultAsync(o => o.ID == asset.ReceivableReport.OrganizationId);

                var printData = new
                {
                    UniqueTagNumber = asset.UniqueTagNumber,
                    AssetTag = asset.AssetTag,
                    OrganizationName = organization?.Name ?? "",
                    OrganizationCode = organization?.Code ?? "",
                    Brand = asset.Brand,
                    Make = asset.Make,
                    Model = asset.Model,
                    SerialNumber = asset.SerialNumber,
                    PrintedDate = currentTime,
                    PrintedBy = currentUser
                };

                return Ok(new { 
                    Message = "Asset tag printed successfully", 
                    AssetId = assetId,
                    UniqueTagNumber = asset.UniqueTagNumber,
                    PrintData = printData 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing asset tag for asset {AssetId}", assetId);
                return StatusCode(500, "An error occurred while printing the asset tag");
            }
        }

        /// <summary>
        /// Attest tag for a specific asset
        /// </summary>
        [HttpPost("asset/{assetId}/attest-tag")]
        public async Task<IActionResult> AttestAssetTag(int assetId)
        {
            try
            {
                var asset = await _context.Assets
                    .FirstOrDefaultAsync(a => a.Id == assetId && !a.IsDeleted);

                if (asset == null)
                {
                    return NotFound($"Asset with ID {assetId} not found");
                }

                if (asset.AssetStatus != "Printed Tag")
                {
                    return BadRequest("Asset tag must be printed before it can be attested");
                }

                var currentUser = GetCurrentUser();
                var currentTime = DateTime.UtcNow;

                // Update asset status to Attested Tag
                asset.AssetStatus = "Attested Tag";
                asset.TagAttestedDate = currentTime;
                asset.TagAttestedBy = currentUser;
                asset.ModifiedDate = currentTime;
                asset.ModifiedBy = currentUser;

                await _context.SaveChangesAsync();

                return Ok(new { 
                    Message = "Asset tag attested successfully", 
                    AssetId = assetId,
                    AttestedDate = currentTime,
                    AttestedBy = currentUser 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attesting asset tag for asset {AssetId}", assetId);
                return StatusCode(500, "An error occurred while attesting the asset tag");
            }
        }

        /// <summary>
        /// Submit a receivable report (change status from Draft to Submitted)
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitReceivableReport(int id)
        {
            try
            {
                var report = await _context.ReceivableReports
                    .FirstOrDefaultAsync(rr => rr.Id == id && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable Report with ID {id} not found");
                }

                // Validate current status
                if (report.RRStatus != "Draft")
                {
                    return BadRequest($"Cannot submit report. Current status is '{report.RRStatus}'. Only 'Draft' reports can be submitted.");
                }

                var currentUser = "System"; // In a real application, get this from the current user context
                var currentTime = DateTime.UtcNow;

                // Update RR status to Submitted
                report.RRStatus = "Submitted";
                report.ModifiedDate = currentTime;
                report.ModifiedBy = currentUser;

                await _context.SaveChangesAsync();

                return Ok(new { 
                    Message = "Receivable Report submitted successfully", 
                    ReportId = id,
                    RRStatus = report.RRStatus,
                    SubmittedDate = currentTime,
                    SubmittedBy = currentUser 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting receivable report {Id}", id);
                return StatusCode(500, "An error occurred while submitting the receivable report");
            }
        }

        /// <summary>
        /// Complete a receivable report (change status to Complete when all assets are attested)
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteReceivableReport(int id)
        {
            try
            {
                var report = await _context.ReceivableReports
                    .Include(rr => rr.Assets.Where(a => !a.IsDeleted))
                    .FirstOrDefaultAsync(rr => rr.Id == id && !rr.IsDeleted);

                if (report == null)
                {
                    return NotFound($"Receivable Report with ID {id} not found");
                }

                // Validate current status
                if (report.RRStatus == "Complete")
                {
                    return BadRequest("Receivable Report is already complete");
                }

                // Validate all assets have been attested
                var nonAttestedAssets = report.Assets.Where(a => a.AssetStatus != "Attested Tag").ToList();
                if (nonAttestedAssets.Any())
                {
                    var assetIds = string.Join(", ", nonAttestedAssets.Select(a => a.Id));
                    return BadRequest($"Cannot complete report. The following assets have not been attested: {assetIds}");
                }

                var currentUser = "System"; // In a real application, get this from the current user context
                var currentTime = DateTime.UtcNow;

                // Update RR status to Complete
                report.RRStatus = "Complete";
                report.CompletedDate = currentTime;
                report.CompletedBy = currentUser;
                report.ModifiedDate = currentTime;
                report.ModifiedBy = currentUser;

                await _context.SaveChangesAsync();

                return Ok(new { 
                    Message = "Receivable Report completed successfully", 
                    ReportId = id,
                    RRStatus = report.RRStatus,
                    CompletedDate = currentTime,
                    CompletedBy = currentUser 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing receivable report {Id}", id);
                return StatusCode(500, "An error occurred while completing the receivable report");
            }
        }

        /// <summary>
        /// Get lookup data for dropdowns (updated to use ID values and FDW database)
        /// </summary>
        [HttpGet("lookup-data")]
        public async Task<ActionResult<ReactAspNetApp.DTOs.LookupDataDto>> GetLookupData()
        {
            try
            {
                // Get FDW reference data (Organizations, Funds, OA1s)
                // Use actual database columns (ID, Organization, Fund, OA1) in queries
                var organizations = await _fdwContext.GL_Organizations
                    .Where(o => o.EffectiveStatus == "A" || o.EffectiveStatus == "1")
                    .Select(o => new { 
                        Id = o.ID,  // Use database column in query
                        Code = o.Organization,  // Actual column name
                        Name = o.OrganizationShortDesc ?? o.OrganizationDesc ?? ""
                    })
                    .ToListAsync();

                var funds = await _fdwContext.GL_Funds
                    .Where(f => f.EffectiveStatus == "A" || f.EffectiveStatus == "1")
                    .Select(f => new { 
                        Id = f.ID, 
                        Code = f.Fund,  // Actual column name
                        Name = f.FundShortDesc ?? f.FundDesc ?? ""
                    })
                    .ToListAsync();

                var oa1s = await _fdwContext.GL_OA1s
                    .Where(o => o.EffectiveStatus == "A" || o.EffectiveStatus == "1")
                    .Select(o => new { 
                        Id = o.ID, 
                        Code = o.OA1,  // Actual column name
                        Name = o.OA1ShortDesc ?? o.OA1Desc ?? ""
                    })
                    .ToListAsync();

                // Get local application data
                var locations = await _context.Locations
                    .Where(l => l.IsActive)
                    .Select(l => new LocationLookupDto 
                    { 
                        Id = l.Id, 
                        Code = l.Code, 
                        AddressLine1 = l.AddressLine1, 
                        City = l.City, 
                        County = l.County, 
                        State = l.State, 
                        PostalCode = l.PostalCode 
                    })
                    .ToListAsync();

                var counties = await _context.Counties
                    .Where(c => c.IsActive)
                    .Select(c => new CountyLookupDto 
                    { 
                        Id = c.Id, 
                        Code = c.Code, 
                        Name = c.Name, 
                        State = c.State 
                    })
                    .ToListAsync();

                var objectCodes = await _context.ObjectCodes
                    .Where(oc => oc.IsActive)
                    .Select(oc => new ObjectCodeLookupDto 
                    { 
                        Id = oc.Id, 
                        Code = oc.Code, 
                        Name = oc.Name 
                    })
                    .ToListAsync();

                var procurementTypes = await _context.ProcurementTypes
                    .Where(pt => pt.IsActive)
                    .Select(pt => new ProcurementTypeLookupDto 
                    { 
                        Id = pt.Id, 
                        Code = pt.Code, 
                        Name = pt.Name, 
                        Description = pt.Description 
                    })
                    .ToListAsync();

                // Get asset groups (only top-level parent groups)
                var childGroupIds = await _context.AssetSubgroups
                    .Where(asg => asg.IsActive)
                    .Select(asg => asg.ChildAssetGroupId)
                    .Distinct()
                    .ToListAsync();

                var assetGroups = await _context.AssetGroups
                    .Where(ag => ag.IsActive && !childGroupIds.Contains(ag.Id))
                    .OrderBy(ag => ag.Name)
                    .Select(ag => new AssetGroupLookupDto 
                    { 
                        Id = ag.Id, 
                        Name = ag.Name, 
                        Description = ag.Description 
                    })
                    .ToListAsync();

                var lookupData = new ReactAspNetApp.DTOs.LookupDataDto
                {
                    Organizations = organizations,
                    Funds = funds,
                    OA1s = oa1s,
                    Locations = locations,
                    Counties = counties,
                    ObjectCodes = objectCodes,
                    ProcurementTypes = procurementTypes,
                    AssetGroups = assetGroups
                };

                return Ok(lookupData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lookup data");
                return StatusCode(500, "An error occurred while retrieving lookup data");
            }
        }

        #region Private Methods

        /// <summary>
        /// Create a history record for tracking changes
        /// </summary>
        private async Task CreateHistoryRecord(ReceivableReport report, string operationType, string? changeReason = null)
        {
            var currentUser = GetCurrentUser();
            var currentTime = DateTime.UtcNow;

            var historyRecord = new ReceivableReportHistory
            {
                ReceivableReportId = report.Id,
                OperationType = operationType,
                OrganizationId = report.OrganizationId,
                FundId = report.FundId,
                OA1Id = report.OA1Id,
                LocationCode = report.LocationCode,
                OrderStatus = report.OrderStatus,
                RRStatus = report.RRStatus,
                AddressLine1 = report.AddressLine1,
                AddressLine2 = report.AddressLine2,
                City = report.City,
                County = report.County,
                State = report.State,
                PostalCode = report.PostalCode,
                // Procurement method reference
                ProcurementMethodId = report.ProcurementMethodId,
                CompletedDate = report.CompletedDate,
                CompletedBy = report.CompletedBy,
                AttestedDate = report.AttestedDate,
                AttestedBy = report.AttestedBy,
                IsDeleted = report.IsDeleted,
                DeletedDate = report.DeletedDate,
                DeletedBy = report.DeletedBy,
                // History tracking fields
                HistoryDate = currentTime,
                HistoryUser = currentUser,
                ChangeReason = changeReason,
                // Original record tracking
                OriginalCreatedDate = report.CreatedDate,
                OriginalCreatedBy = report.CreatedBy,
                OriginalModifiedDate = report.ModifiedDate,
                OriginalModifiedBy = report.ModifiedBy
            };

            _context.ReceivableReportHistory.Add(historyRecord);
            await _context.SaveChangesAsync();
        }

        private async Task<List<ReceivableReportDto>> MapToDtoListAsync(List<ReceivableReport> reports)
        {
            if (!reports.Any()) return new List<ReceivableReportDto>();

            // Get all unique IDs for bulk lookup
            var orgIds = reports.Select(r => r.OrganizationId).Distinct().ToList();
            var fundIds = reports.Select(r => r.FundId).Distinct().ToList();
            var oa1Ids = reports.Select(r => r.OA1Id).Distinct().ToList();

            // Bulk lookup FDW data (use ID which is the actual database column, not the unmapped Id property)
            var organizations = await _fdwContext.GL_Organizations
                .Where(o => orgIds.Contains(o.ID))
                .ToDictionaryAsync(o => o.ID, o => o);
            
            var funds = await _fdwContext.GL_Funds
                .Where(f => fundIds.Contains(f.ID))
                .ToDictionaryAsync(f => f.ID, f => f);
            
            var oa1s = await _fdwContext.GL_OA1s
                .Where(o => oa1Ids.Contains(o.ID))
                .ToDictionaryAsync(o => o.ID, o => o);

            // Map to DTOs
            return reports.Select(report => new ReceivableReportDto
            {
                Id = report.Id,
                OrganizationId = report.OrganizationId,
                OrganizationCode = organizations.GetValueOrDefault(report.OrganizationId)?.Code ?? "",
                OrganizationName = organizations.GetValueOrDefault(report.OrganizationId)?.Name ?? "",
                FundId = report.FundId,
                FundCode = funds.GetValueOrDefault(report.FundId)?.Code ?? "",
                FundName = funds.GetValueOrDefault(report.FundId)?.Name ?? "",
                OA1Id = report.OA1Id,
                OA1Code = oa1s.GetValueOrDefault(report.OA1Id)?.Code ?? "",
                OA1Name = oa1s.GetValueOrDefault(report.OA1Id)?.Name ?? "",
                LocationCode = report.LocationCode,
                OrderStatus = report.OrderStatus,
                RRStatus = report.RRStatus,
                AddressLine1 = report.AddressLine1,
                AddressLine2 = report.AddressLine2,
                City = report.City,
                County = report.County,
                State = report.State,
                PostalCode = report.PostalCode,
                ProcurementMethodId = report.ProcurementMethodId,
                ProcurementMethod = report.ProcurementMethod != null ? new ProcurementMethodDto
                {
                    Id = report.ProcurementMethod.Id,
                    ProcurementTypeId = report.ProcurementMethod.ProcurementTypeId,
                    ProcurementTypeCode = report.ProcurementMethod.ProcurementTypeCode,
                    ProcurementTypeName = report.ProcurementMethod.ProcurementType?.Name ?? "",
                    ChargeDate = report.ProcurementMethod.ChargeDate,
                    PcardHolderFirstName = report.ProcurementMethod.PcardHolderFirstName,
                    PcardHolderLastName = report.ProcurementMethod.PcardHolderLastName,
                    GroupId = report.ProcurementMethod.GroupId,
                    PurchaseOrderNumber = report.ProcurementMethod.PurchaseOrderNumber,
                    ContractNumber = report.ProcurementMethod.ContractNumber,
                    CreatedDate = report.ProcurementMethod.CreatedDate,
                    CreatedBy = report.ProcurementMethod.CreatedBy,
                    ModifiedDate = report.ProcurementMethod.ModifiedDate,
                    ModifiedBy = report.ProcurementMethod.ModifiedBy
                } : null,
                CreatedDate = report.CreatedDate,
                CreatedBy = report.CreatedBy,
                ModifiedDate = report.ModifiedDate,
                ModifiedBy = report.ModifiedBy,
                CompletedDate = report.CompletedDate,
                CompletedBy = report.CompletedBy,
                AttestedDate = report.AttestedDate,
                AttestedBy = report.AttestedBy,
                Assets = report.Assets?.Select(MapAssetToDto).ToList() ?? new List<AssetDto>(),
                AssetCount = report.Assets?.Count ?? 0,
                TotalAssetValue = report.Assets?.Sum(a => a.AssetValue) ?? 0
            }).ToList();
        }

        private async Task<ReceivableReportDto> MapToDtoAsync(ReceivableReport report)
        {
            // Lookup FDW data (use ID which is the actual database column)
            var organization = await _fdwContext.GL_Organizations
                .Where(o => o.ID == report.OrganizationId)
                .FirstOrDefaultAsync();
            
            var fund = await _fdwContext.GL_Funds
                .Where(f => f.ID == report.FundId)
                .FirstOrDefaultAsync();
            
            var oa1 = await _fdwContext.GL_OA1s
                .Where(o => o.ID == report.OA1Id)
                .FirstOrDefaultAsync();

            return new ReceivableReportDto
            {
                Id = report.Id,
                OrganizationId = report.OrganizationId,
                OrganizationCode = organization?.Code ?? "",
                OrganizationName = organization?.Name ?? "",
                FundId = report.FundId,
                FundCode = fund?.Code ?? "",
                FundName = fund?.Name ?? "",
                OA1Id = report.OA1Id,
                OA1Code = oa1?.Code ?? "",
                OA1Name = oa1?.Name ?? "",
                LocationId=report.LocationId,
                LocationCode = report.LocationCode,
                OrderStatus = report.OrderStatus,
                RRStatus = report.RRStatus,
                AddressLine1 = report.AddressLine1,
                AddressLine2 = report.AddressLine2,
                City = report.City,
                County = report.County,
                State = report.State,
                PostalCode = report.PostalCode,
                // Procurement method
                ProcurementMethodId = report.ProcurementMethodId,
                ProcurementMethod = report.ProcurementMethod != null ? new ProcurementMethodDto
                {
                    Id = report.ProcurementMethod.Id,
                    ProcurementTypeId = report.ProcurementMethod.ProcurementTypeId,
                    ProcurementTypeCode = report.ProcurementMethod.ProcurementTypeCode,
                    ProcurementTypeName = report.ProcurementMethod.ProcurementType?.Name ?? "",
                    ChargeDate = report.ProcurementMethod.ChargeDate,
                    PcardHolderFirstName = report.ProcurementMethod.PcardHolderFirstName,
                    PcardHolderLastName = report.ProcurementMethod.PcardHolderLastName,
                    GroupId = report.ProcurementMethod.GroupId,
                    PurchaseOrderNumber = report.ProcurementMethod.PurchaseOrderNumber,
                    ContractNumber = report.ProcurementMethod.ContractNumber,
                    CreatedDate = report.ProcurementMethod.CreatedDate,
                    CreatedBy = report.ProcurementMethod.CreatedBy,
                    ModifiedDate = report.ProcurementMethod.ModifiedDate,
                    ModifiedBy = report.ProcurementMethod.ModifiedBy
                } : null,
                CreatedDate = report.CreatedDate,
                CreatedBy = report.CreatedBy,
                ModifiedDate = report.ModifiedDate,
                ModifiedBy = report.ModifiedBy,
                CompletedDate = report.CompletedDate,
                CompletedBy = report.CompletedBy,
                AttestedDate = report.AttestedDate,
                AttestedBy = report.AttestedBy,
                Assets = report.Assets?.Select(MapAssetToDto).ToList() ?? new List<AssetDto>(),
                AssetCount = report.Assets?.Count ?? 0,
                TotalAssetValue = report.Assets?.Sum(a => a.AssetValue) ?? 0
            };
        }

        private AssetDto MapAssetToDto(Asset asset)
        {
            return new AssetDto
            {
                Id = asset.Id,
                ReceivableReportId = asset.ReceivableReportId,
                Brand = asset.Brand ?? string.Empty,
                Make = asset.Make ?? string.Empty,
                Model = asset.Model ?? string.Empty,
                AssetTag = asset.AssetTag ?? string.Empty,
                SerialNumber = asset.SerialNumber ?? string.Empty,
                AssetValue = asset.AssetValue,
                ObjectCodeId = asset.ObjectCodeId,
                ObjectCodeName = asset.ObjectCodeNavigation?.Name,
                AssetGroupId = asset.AssetGroupId,
                AssetGroupName = asset.AssetGroup?.Name,
                AssetSubGroupId = asset.AssetSubGroupId,
                AssetSubGroupName = asset.AssetSubGroup?.Name,
                AssignedTo = asset.AssignedTo,
                Floor = asset.Floor,
                Room = asset.Room,
                IsOwnedByCounty = asset.IsOwnedByCounty,
                CountyId = asset.CountyId,
                CountyName = asset.County?.Name,
                UniqueTagNumber = asset.UniqueTagNumber,
                AssetStatus = asset.AssetStatus ?? "Open",
                TagPrintedDate = asset.TagPrintedDate,
                TagPrintedBy = asset.TagPrintedBy,
                TagAttestedDate = asset.TagAttestedDate,
                TagAttestedBy = asset.TagAttestedBy,
                CreatedDate = asset.CreatedDate,
                CreatedBy = asset.CreatedBy ?? string.Empty,
                ModifiedDate = asset.ModifiedDate,
                ModifiedBy = asset.ModifiedBy
            };
        }

        private string GetCurrentUser()
        {
            // In a real application, this would get the current user from the authentication context
            return "System"; // Placeholder
        }

        private string GenerateUniqueTagNumber(string organizationCode)
        {
            // Generate a unique tag number: ORG-YYYY-MMDD-HHMMSS-RND
            var now = DateTime.UtcNow;
            var random = new Random().Next(100, 999);
            return $"{organizationCode}-{now:yyyy}-{now:MMdd}-{now:HHmmss}-{random}";
        }

        #endregion
    }

    #region Supporting Classes

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }

    // LookupDataDto moved to ReactAspNetApp.DTOs namespace

    #endregion
}
