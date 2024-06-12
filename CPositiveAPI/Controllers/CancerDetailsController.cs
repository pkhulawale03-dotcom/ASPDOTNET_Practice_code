using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CPositiveAPI.Controllers.CPatientController;

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

        [HttpPost("CancerDetails")]      
        public IActionResult AddCancerDetails([FromBody] CreateCancerDetails model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddDetails(model);
                    return Ok(new { StatusCode = 200, Message = "Cancer Details Added Successfully", Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddDetails(CreateCancerDetails newUser)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {                            
                var Cancerdtls = new CancerInfo
                {                   
                    UserId = newUser.UserId,
                    CancertypeId = newUser.CancertypeId,
                    CancerNameId = newUser.CancerNameId,
                    StageId = newUser.StageId,
                    GradeId = newUser.GradeId,
                    IsFirstTime = newUser.IsFirstTime,
                    IsRelapsed = newUser.IsRelapsed,
                    IsTreatmentOngoing = newUser.IsTreatmentOngoing,
                    IsSurgery = newUser.IsSurgery,
                    IsChemo = newUser.IsChemo,
                    IsRadiation = newUser.IsRadiation,
                    IsTargetedTherapy = newUser.IsTargetedTherapy,
                    IsPallitiveCare = newUser.IsPallitiveCare,
                    IsRemission = newUser.IsRemission,
                    Createdon = DateTime.Now
                };

                Context.CancerInfo.Add(Cancerdtls);
                Context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public class CreateCancerDetails
        {           
            public int UserId { get; set; }
            public int CancertypeId { get; set; }
            public int CancerNameId { get; set; }
            public int StageId { get; set; }
            public int GradeId { get; set; }
            public string IsFirstTime { get; set; }
            public string IsRelapsed { get; set; }
            public string IsTreatmentOngoing { get; set; }
            public string IsSurgery { get; set; }
            public string IsChemo { get; set; }
            public string IsRadiation { get; set; }
            public string IsTargetedTherapy { get; set; }
            public string IsPallitiveCare { get; set; }
            public string IsRemission { get; set; }
        }
    }
}
