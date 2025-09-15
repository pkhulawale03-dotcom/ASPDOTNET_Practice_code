using CPositiveAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        public TestController(ApplicationDbContext applicationDbContext)
        {
            applicationDbContext = Context;
        }

        [HttpGet]
        public ActionResult get() 
        {
            try
            {
                var getdata= Context.PatientDetails.Include(p => p.PatientId).ToList();
                if(getdata == null)
                {
                    return NotFound();
                }
                return Ok(getdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
