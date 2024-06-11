using CPositiveAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancerDetailsController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        public CancerDetailsController(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        [HttpGet("CancerName")]
        public IActionResult GetCancerName()
        {
            var cancername = Context.CancerNameMaster.ToList();
            return Ok(cancername);
        }

        [HttpGet("CancerType")]
        public IActionResult GetCancerType()
        {
            var cancertype = Context.CancerTypesMaster.ToList();
            return Ok(cancertype);
        }

        [HttpGet("CancerGrades")]
        public IActionResult GetCancerGrades()
        {
            var cancergrade = Context.GradeMaster.ToList();
            return Ok(cancergrade);
        }

        [HttpGet("CancerStages")]
        public IActionResult GetCancerStages()
        {
            var cancerstage = Context.StageMaster.ToList();
            return Ok(cancerstage);
        }

     
    }
}
