using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.Data;
using ReactAspNetApp.Models;
using ReactAspNetApp.DTOs;
using System.Linq;

namespace ReactAspNetApp.Controllers
{
    /// <summary>
    /// API Controller for managing Application Security
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationSecurityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationSecurityController> _logger;

        public ApplicationSecurityController(ApplicationDbContext context, ILogger<ApplicationSecurityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Search for application security records with filtering and pagination
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<PagedResult<ApplicationSecurityDto>>> SearchApplicationSecurity([FromBody] ApplicationSecuritySearchDto searchDto)
        {
            try
            {
                var query = _context.ApplicationSecurity
                    .Include(a => a.ApplicationRole)
                    .AsQueryable();

                // Apply filters
                if (!searchDto.IncludeDeleted)
                {
                    query = query.Where(a => a.IsActive);
                }
                else if (searchDto.IsActive.HasValue)
                {
                    query = query.Where(a => a.IsActive == searchDto.IsActive.Value);
                }

                // Filter by Username (email)
                if (!string.IsNullOrEmpty(searchDto.Username))
                {
                    query = query.Where(a => a.Username.Contains(searchDto.Username));
                }

                // Filter by Application Role ID
                if (searchDto.ApplicationRoleId.HasValue)
                {
                    query = query.Where(a => a.ApplicationRoleId == searchDto.ApplicationRoleId.Value);
                }
                // Filter by Application Role Name
                else if (!string.IsNullOrEmpty(searchDto.ApplicationRoleName))
                {
                    query = query.Where(a => a.ApplicationRole.Name.Contains(searchDto.ApplicationRoleName));
                }

                // Filter by Created Date range
                if (searchDto.CreatedDateFrom.HasValue)
                {
                    query = query.Where(a => a.CreatedDate >= searchDto.CreatedDateFrom.Value);
                }
                if (searchDto.CreatedDateTo.HasValue)
                {
                    query = query.Where(a => a.CreatedDate <= searchDto.CreatedDateTo.Value);
                }

                // Filter by Created By
                if (!string.IsNullOrEmpty(searchDto.CreatedBy))
                {
                    query = query.Where(a => a.CreatedBy.Contains(searchDto.CreatedBy));
                }

                // Filter by Updated Date range
                if (searchDto.UpdatedDateFrom.HasValue)
                {
                    query = query.Where(a => a.UpdatedDate.HasValue && a.UpdatedDate >= searchDto.UpdatedDateFrom.Value);
                }
                if (searchDto.UpdatedDateTo.HasValue)
                {
                    query = query.Where(a => a.UpdatedDate.HasValue && a.UpdatedDate <= searchDto.UpdatedDateTo.Value);
                }

                // Filter by Updated By
                if (!string.IsNullOrEmpty(searchDto.UpdatedBy))
                {
                    query = query.Where(a => a.UpdatedBy != null && a.UpdatedBy.Contains(searchDto.UpdatedBy));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply sorting
                var sortBy = searchDto.SortBy ?? "CreatedDate";
                var sortDirection = searchDto.SortDirection ?? "DESC";

                query = sortDirection.ToUpper() == "ASC"
                    ? sortBy switch
                    {
                        "Username" => query.OrderBy(a => a.Username),
                        "ApplicationRole" => query.OrderBy(a => a.ApplicationRole.Name),
                        "ApplicationRoleName" => query.OrderBy(a => a.ApplicationRole.Name),
                        "CreatedDate" => query.OrderBy(a => a.CreatedDate),
                        "CreatedBy" => query.OrderBy(a => a.CreatedBy),
                        "UpdatedDate" => query.OrderBy(a => a.UpdatedDate),
                        "UpdatedBy" => query.OrderBy(a => a.UpdatedBy),
                        _ => query.OrderBy(a => a.CreatedDate)
                    }
                    : sortBy switch
                    {
                        "Username" => query.OrderByDescending(a => a.Username),
                        "ApplicationRole" => query.OrderByDescending(a => a.ApplicationRole.Name),
                        "ApplicationRoleName" => query.OrderByDescending(a => a.ApplicationRole.Name),
                        "CreatedDate" => query.OrderByDescending(a => a.CreatedDate),
                        "CreatedBy" => query.OrderByDescending(a => a.CreatedBy),
                        "UpdatedDate" => query.OrderByDescending(a => a.UpdatedDate),
                        "UpdatedBy" => query.OrderByDescending(a => a.UpdatedBy),
                        _ => query.OrderByDescending(a => a.CreatedDate)
                    };

                // Apply pagination
                var pageNumber = searchDto.PageNumber > 0 ? searchDto.PageNumber : 1;
                var pageSize = searchDto.PageSize > 0 ? searchDto.PageSize : 10;
                var skip = (pageNumber - 1) * pageSize;

                var items = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(a => new ApplicationSecurityDto
                    {
                        Id = a.Id,
                        Username = a.Username,
                        ApplicationRoleId = a.ApplicationRoleId,
                        ApplicationRoleName = a.ApplicationRole.Name,
                        ApplicationRole = new ApplicationRoleDto
                        {
                            Id = a.ApplicationRole.Id,
                            Name = a.ApplicationRole.Name,
                            Description = a.ApplicationRole.Description,
                            CreatedDate = a.ApplicationRole.CreatedDate,
                            CreatedBy = a.ApplicationRole.CreatedBy,
                            UpdatedDate = a.ApplicationRole.UpdatedDate,
                            UpdatedBy = a.ApplicationRole.UpdatedBy,
                            IsActive = a.ApplicationRole.IsActive
                        },
                        CreatedDate = a.CreatedDate,
                        CreatedBy = a.CreatedBy,
                        UpdatedDate = a.UpdatedDate,
                        UpdatedBy = a.UpdatedBy,
                        IsActive = a.IsActive
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var result = new PagedResult<ApplicationSecurityDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching application security records");
                return StatusCode(500, "An error occurred while searching application security records");
            }
        }

        /// <summary>
        /// Get all application security records
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationSecurityDto>>> GetAllApplicationSecurity()
        {
            try
            {
                var records = await _context.ApplicationSecurity
                    .Include(a => a.ApplicationRole)
                    .Where(a => a.IsActive)
                    .OrderBy(a => a.Username)
                    .Select(a => new ApplicationSecurityDto
                    {
                        Id = a.Id,
                        Username = a.Username,
                        ApplicationRoleId = a.ApplicationRoleId,
                        ApplicationRoleName = a.ApplicationRole.Name,
                        ApplicationRole = new ApplicationRoleDto
                        {
                            Id = a.ApplicationRole.Id,
                            Name = a.ApplicationRole.Name,
                            Description = a.ApplicationRole.Description,
                            CreatedDate = a.ApplicationRole.CreatedDate,
                            CreatedBy = a.ApplicationRole.CreatedBy,
                            UpdatedDate = a.ApplicationRole.UpdatedDate,
                            UpdatedBy = a.ApplicationRole.UpdatedBy,
                            IsActive = a.ApplicationRole.IsActive
                        },
                        CreatedDate = a.CreatedDate,
                        CreatedBy = a.CreatedBy,
                        UpdatedDate = a.UpdatedDate,
                        UpdatedBy = a.UpdatedBy,
                        IsActive = a.IsActive
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application security records");
                return StatusCode(500, "An error occurred while retrieving application security records");
            }
        }

        /// <summary>
        /// Get application security record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationSecurityDto>> GetApplicationSecurity(int id)
        {
            try
            {
                var record = await _context.ApplicationSecurity
                    .Include(a => a.ApplicationRole)
                    .Where(a => a.Id == id)
                    .Select(a => new ApplicationSecurityDto
                    {
                        Id = a.Id,
                        Username = a.Username,
                        ApplicationRoleId = a.ApplicationRoleId,
                        ApplicationRoleName = a.ApplicationRole.Name,
                        ApplicationRole = new ApplicationRoleDto
                        {
                            Id = a.ApplicationRole.Id,
                            Name = a.ApplicationRole.Name,
                            Description = a.ApplicationRole.Description,
                            CreatedDate = a.ApplicationRole.CreatedDate,
                            CreatedBy = a.ApplicationRole.CreatedBy,
                            UpdatedDate = a.ApplicationRole.UpdatedDate,
                            UpdatedBy = a.ApplicationRole.UpdatedBy,
                            IsActive = a.ApplicationRole.IsActive
                        },
                        CreatedDate = a.CreatedDate,
                        CreatedBy = a.CreatedBy,
                        UpdatedDate = a.UpdatedDate,
                        UpdatedBy = a.UpdatedBy,
                        IsActive = a.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (record == null)
                {
                    return NotFound($"Application security record with ID {id} not found");
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application security record with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the application security record");
            }
        }

        /// <summary>
        /// Create a new application security record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApplicationSecurityDto>> CreateApplicationSecurity([FromBody] ApplicationSecurityDto dto)
        {
            try
            {
                // Validate email format
                var emailAttribute = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
                if (!emailAttribute.IsValid(dto.Username))
                {
                    return BadRequest("Username must be a valid email address");
                }

                // Check if username already exists
                var existing = await _context.ApplicationSecurity
                    .FirstOrDefaultAsync(a => a.Username == dto.Username);
                
                if (existing != null)
                {
                    return BadRequest("Username already exists");
                }

                // Validate ApplicationRoleId exists
                var roleExists = await _context.ApplicationRoles
                    .AnyAsync(r => r.Id == dto.ApplicationRoleId && r.IsActive);
                
                if (!roleExists)
                {
                    return BadRequest("Invalid ApplicationRoleId. The role does not exist or is not active.");
                }

                var record = new ApplicationSecurity
                {
                    Username = dto.Username,
                    ApplicationRoleId = dto.ApplicationRoleId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy ?? "System",
                    IsActive = dto.IsActive
                };

                _context.ApplicationSecurity.Add(record);
                await _context.SaveChangesAsync();

                // Reload with role information
                await _context.Entry(record).Reference(a => a.ApplicationRole).LoadAsync();

                var result = new ApplicationSecurityDto
                {
                    Id = record.Id,
                    Username = record.Username,
                    ApplicationRoleId = record.ApplicationRoleId,
                    ApplicationRoleName = record.ApplicationRole.Name,
                    ApplicationRole = new ApplicationRoleDto
                    {
                        Id = record.ApplicationRole.Id,
                        Name = record.ApplicationRole.Name,
                        Description = record.ApplicationRole.Description,
                        CreatedDate = record.ApplicationRole.CreatedDate,
                        CreatedBy = record.ApplicationRole.CreatedBy,
                        UpdatedDate = record.ApplicationRole.UpdatedDate,
                        UpdatedBy = record.ApplicationRole.UpdatedBy,
                        IsActive = record.ApplicationRole.IsActive
                    },
                    CreatedDate = record.CreatedDate,
                    CreatedBy = record.CreatedBy,
                    UpdatedDate = record.UpdatedDate,
                    UpdatedBy = record.UpdatedBy,
                    IsActive = record.IsActive
                };

                return CreatedAtAction(nameof(GetApplicationSecurity), new { id = record.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating application security record");
                return StatusCode(500, "An error occurred while creating the application security record");
            }
        }

        /// <summary>
        /// Update an existing application security record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationSecurityDto>> UpdateApplicationSecurity(int id, [FromBody] ApplicationSecurityDto dto)
        {
            try
            {
                var record = await _context.ApplicationSecurity.FindAsync(id);
                if (record == null)
                {
                    return NotFound($"Application security record with ID {id} not found");
                }

                // Validate email format
                var emailAttribute = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
                if (!emailAttribute.IsValid(dto.Username))
                {
                    return BadRequest("Username must be a valid email address");
                }

                // Check if username already exists for a different record
                var existing = await _context.ApplicationSecurity
                    .FirstOrDefaultAsync(a => a.Username == dto.Username && a.Id != id);
                
                if (existing != null)
                {
                    return BadRequest("Username already exists");
                }

                // Validate ApplicationRoleId exists
                var roleExists = await _context.ApplicationRoles
                    .AnyAsync(r => r.Id == dto.ApplicationRoleId && r.IsActive);
                
                if (!roleExists)
                {
                    return BadRequest("Invalid ApplicationRoleId. The role does not exist or is not active.");
                }

                record.Username = dto.Username;
                record.ApplicationRoleId = dto.ApplicationRoleId;
                record.UpdatedDate = DateTime.UtcNow;
                record.UpdatedBy = dto.UpdatedBy ?? "System";
                record.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                // Reload with role information
                await _context.Entry(record).Reference(a => a.ApplicationRole).LoadAsync();

                var result = new ApplicationSecurityDto
                {
                    Id = record.Id,
                    Username = record.Username,
                    ApplicationRoleId = record.ApplicationRoleId,
                    ApplicationRoleName = record.ApplicationRole.Name,
                    ApplicationRole = new ApplicationRoleDto
                    {
                        Id = record.ApplicationRole.Id,
                        Name = record.ApplicationRole.Name,
                        Description = record.ApplicationRole.Description,
                        CreatedDate = record.ApplicationRole.CreatedDate,
                        CreatedBy = record.ApplicationRole.CreatedBy,
                        UpdatedDate = record.ApplicationRole.UpdatedDate,
                        UpdatedBy = record.ApplicationRole.UpdatedBy,
                        IsActive = record.ApplicationRole.IsActive
                    },
                    CreatedDate = record.CreatedDate,
                    CreatedBy = record.CreatedBy,
                    UpdatedDate = record.UpdatedDate,
                    UpdatedBy = record.UpdatedBy,
                    IsActive = record.IsActive
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application security record with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the application security record");
            }
        }

        /// <summary>
        /// Delete an application security record (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationSecurity(int id)
        {
            try
            {
                var record = await _context.ApplicationSecurity.FindAsync(id);
                if (record == null)
                {
                    return NotFound($"Application security record with ID {id} not found");
                }

                // Soft delete
                record.IsActive = false;
                record.UpdatedDate = DateTime.UtcNow;
                record.UpdatedBy = "System"; // In a real app, get from current user context

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting application security record with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the application security record");
            }
        }

        /// <summary>
        /// Get all available application roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<ApplicationRoleDto>>> GetApplicationRoles()
        {
            try
            {
                var roles = await _context.ApplicationRoles
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Name)
                    .Select(r => new ApplicationRoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CreatedDate = r.CreatedDate,
                        CreatedBy = r.CreatedBy,
                        UpdatedDate = r.UpdatedDate,
                        UpdatedBy = r.UpdatedBy,
                        IsActive = r.IsActive
                    })
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application roles");
                return StatusCode(500, "An error occurred while retrieving application roles");
            }
        }
    }
}

