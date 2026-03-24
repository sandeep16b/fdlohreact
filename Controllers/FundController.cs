using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.FDWData;

namespace ReactAspNetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundController : ControllerBase
    {
        private readonly FDWDbContext _fdwContext;
        private readonly ILogger<FundController> _logger;

        public FundController(FDWDbContext fdwContext, ILogger<FundController> logger)
        {
            _fdwContext = fdwContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFunds()
        {
            try
            {
                var funds = await _fdwContext.GL_Funds
                    .Where(f => f.EffectiveStatus == "A" || f.EffectiveStatus == "1")
                    .Select(f => new
                    {
                        Id = f.ID,  // Use actual database column
                        Code = f.Fund,  // Use actual database column
                        Name = f.FundShortDesc ?? f.FundDesc ?? ""
                    })
                    .OrderBy(f => f.Name)
                    .ToListAsync();

                return Ok(funds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving funds");
                return StatusCode(500, "An error occurred while retrieving funds");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFund(int id)
        {
            try
            {
                var fund = await _fdwContext.GL_Funds
                    .Where(f => f.ID == id && (f.EffectiveStatus == "A" || f.EffectiveStatus == "1"))
                    .Select(f => new
                    {
                        Id = f.ID,
                        Code = f.Fund,
                        Name = f.FundShortDesc ?? f.FundDesc ?? ""
                    })
                    .FirstOrDefaultAsync();

                if (fund == null)
                {
                    return NotFound($"Fund with ID {id} not found");
                }

                return Ok(fund);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fund with ID {FundId}", id);
                return StatusCode(500, "An error occurred while retrieving the fund");
            }
        }
    }
}
