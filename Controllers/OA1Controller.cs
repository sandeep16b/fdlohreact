using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.FDWData;

namespace ReactAspNetApp.Controllers
{
    
    [Route("api/[controller]")]
    public class OA1Controller : ApiBaseController
    {
        private readonly FDWDbContext _fdwContext;
        private readonly ILogger<OA1Controller> _logger;

        public OA1Controller(FDWDbContext fdwContext, ILogger<OA1Controller> logger)
        {
            _fdwContext = fdwContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetOA1s()
        {
            try
            {
                var oa1s = await _fdwContext.GL_OA1s
                    .Where(o => o.EffectiveStatus == "A" || o.EffectiveStatus == "1")
                    .Select(o => new
                    {
                        Id = o.ID,  // Use actual database column
                        Code = o.OA1,  // Use actual database column
                        Name = o.OA1ShortDesc ?? o.OA1Desc ?? ""
                    })
                    .OrderBy(o => o.Name)
                    .ToListAsync();

                return Ok(oa1s);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving OA1s");
                return StatusCode(500, "An error occurred while retrieving OA1s");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetOA1(int id)
        {
            try
            {
                var oa1 = await _fdwContext.GL_OA1s
                    .Where(o => o.ID == id && (o.EffectiveStatus == "A" || o.EffectiveStatus == "1"))
                    .Select(o => new
                    {
                        Id = o.ID,
                        Code = o.OA1,
                        Name = o.OA1ShortDesc ?? o.OA1Desc ?? ""
                    })
                    .FirstOrDefaultAsync();

                if (oa1 == null)
                {
                    return NotFound($"OA1 with ID {id} not found");
                }

                return Ok(oa1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving OA1 with ID {OA1Id}", id);
                return StatusCode(500, "An error occurred while retrieving the OA1");
            }
        }
    }
}
