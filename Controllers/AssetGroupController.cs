using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.Data;
using ReactAspNetApp.Models;

namespace ReactAspNetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetGroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AssetGroupController> _logger;

        public AssetGroupController(ApplicationDbContext context, ILogger<AssetGroupController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active asset groups
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAssetGroups()
        {
            try
            {
                var assetGroups = await _context.AssetGroups
                    .Where(ag => ag.IsActive)
                    .OrderBy(ag => ag.Name)
                    .Select(ag => new
                    {
                        Id = ag.Id,
                        Name = ag.Name,
                        Description = ag.Description,
                        CreatedDate = ag.CreatedDate,
                        UpdatedDate = ag.UpdatedDate
                    })
                    .ToListAsync();

                return Ok(assetGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset groups");
                return StatusCode(500, "An error occurred while retrieving asset groups");
            }
        }

        /// <summary>
        /// Get asset group by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAssetGroup(int id)
        {
            try
            {
                var assetGroup = await _context.AssetGroups
                    .Where(ag => ag.Id == id && ag.IsActive)
                    .Select(ag => new
                    {
                        Id = ag.Id,
                        Name = ag.Name,
                        Description = ag.Description,
                        CreatedDate = ag.CreatedDate,
                        UpdatedDate = ag.UpdatedDate
                    })
                    .FirstOrDefaultAsync();

                if (assetGroup == null)
                {
                    return NotFound($"Asset group with ID {id} not found");
                }

                return Ok(assetGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset group with ID {AssetGroupId}", id);
                return StatusCode(500, "An error occurred while retrieving the asset group");
            }
        }

        /// <summary>
        /// Get subgroups for a parent asset group
        /// </summary>
        [HttpGet("{parentId}/subgroups")]
        public async Task<ActionResult<IEnumerable<object>>> GetSubgroups(int parentId)
        {
            try
            {
                var subgroups = await _context.AssetSubgroups
                    .Where(asg => asg.ParentAssetGroupId == parentId && asg.IsActive)
                    .Include(asg => asg.ChildAssetGroup)
                    .Where(asg => asg.ChildAssetGroup.IsActive)
                    .OrderBy(asg => asg.ChildAssetGroup.Name)
                    .Select(asg => new
                    {
                        Id = asg.ChildAssetGroup.Id,
                        Name = asg.ChildAssetGroup.Name,
                        Description = asg.ChildAssetGroup.Description,
                        CreatedDate = asg.ChildAssetGroup.CreatedDate,
                        UpdatedDate = asg.ChildAssetGroup.UpdatedDate
                    })
                    .ToListAsync();

                return Ok(subgroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subgroups for parent asset group {ParentId}", parentId);
                return StatusCode(500, "An error occurred while retrieving subgroups");
            }
        }

        /// <summary>
        /// Get all parent asset groups (groups that have no parents themselves)
        /// </summary>
        [HttpGet("parents")]
        public async Task<ActionResult<IEnumerable<object>>> GetParentGroups()
        {
            try
            {
                // Get all group IDs that are children of other groups
                var childGroupIds = await _context.AssetSubgroups
                    .Where(asg => asg.IsActive)
                    .Select(asg => asg.ChildAssetGroupId)
                    .Distinct()
                    .ToListAsync();

                // Get groups that are NOT in the child list (i.e., top-level parents)
                var parentGroups = await _context.AssetGroups
                    .Where(ag => ag.IsActive && !childGroupIds.Contains(ag.Id))
                    .OrderBy(ag => ag.Name)
                    .Select(ag => new
                    {
                        Id = ag.Id,
                        Name = ag.Name,
                        Description = ag.Description,
                        CreatedDate = ag.CreatedDate,
                        UpdatedDate = ag.UpdatedDate
                    })
                    .ToListAsync();

                return Ok(parentGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent asset groups");
                return StatusCode(500, "An error occurred while retrieving parent asset groups");
            }
        }

        /// <summary>
        /// Get the hierarchy of asset groups with their relationships
        /// </summary>
        [HttpGet("hierarchy")]
        public async Task<ActionResult<object>> GetAssetGroupHierarchy()
        {
            try
            {
                // Get all active asset groups
                var allGroups = await _context.AssetGroups
                    .Where(ag => ag.IsActive)
                    .Select(ag => new
                    {
                        Id = ag.Id,
                        Name = ag.Name,
                        Description = ag.Description
                    })
                    .ToDictionaryAsync(ag => ag.Id);

                // Get all active relationships
                var relationships = await _context.AssetSubgroups
                    .Where(asg => asg.IsActive)
                    .Select(asg => new
                    {
                        ParentId = asg.ParentAssetGroupId,
                        ChildId = asg.ChildAssetGroupId
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Groups = allGroups.Values,
                    Relationships = relationships
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset group hierarchy");
                return StatusCode(500, "An error occurred while retrieving asset group hierarchy");
            }
        }
    }
}

