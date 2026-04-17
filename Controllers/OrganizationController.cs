using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.FDWData;

namespace ReactAspNetApp.Controllers
{
    
    [Route("api/[controller]")]
    public class OrganizationController : ApiBaseController
    {
        private readonly FDWDbContext _fdwContext;
        private readonly ILogger<OrganizationController> _logger;

        public OrganizationController(FDWDbContext fdwContext, ILogger<OrganizationController> logger)
        {
            _fdwContext = fdwContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetOrganizations()
        {
            try
            {
                var organizations = await _fdwContext.GL_Organizations
                    .Where(o => o.EffectiveStatus == "A" || o.EffectiveStatus == "1")
                    .Select(o => new
                    {
                        Id = o.ID,  // Use actual database column
                        Code = o.Organization,  // Use actual database column
                        Name = o.OrganizationShortDesc ?? o.OrganizationDesc ?? ""
                    })
                    .OrderBy(o => o.Name)
                    .ToListAsync();

                return Ok(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organizations");
                return StatusCode(500, "An error occurred while retrieving organizations");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetOrganization(int id)
        {
            try
            {
                var organization = await _fdwContext.GL_Organizations
                    .Where(o => o.ID == id && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"))
                    .Select(o => new
                    {
                        Id = o.ID,
                        Code = o.Organization,
                        Name = o.OrganizationShortDesc ?? o.OrganizationDesc ?? ""
                    })
                    .FirstOrDefaultAsync();

                if (organization == null)
                {
                    return NotFound($"Organization with ID {id} not found");
                }

                return Ok(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization with ID {OrganizationId}", id);
                return StatusCode(500, "An error occurred while retrieving the organization");
            }
        }

        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<object>> GetOrganizationByCode(string code)
        {
            try
            {
                var organization = await _fdwContext.GL_Organizations
                    .Where(o => o.Organization == code && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"))
                    .Select(o => new
                    {
                        Id = o.ID,
                        Code = o.Organization,
                        Name = o.OrganizationShortDesc ?? o.OrganizationDesc ?? ""
                    })
                    .FirstOrDefaultAsync();

                if (organization == null)
                {
                    return NotFound($"Organization with code {code} not found");
                }

                return Ok(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization with code {OrganizationCode}", code);
                return StatusCode(500, "An error occurred while retrieving the organization");
            }
        }
    }
}
