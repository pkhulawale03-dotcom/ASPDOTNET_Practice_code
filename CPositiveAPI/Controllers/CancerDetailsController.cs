using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        private void AddDetails(CreateCancerDetails cancerdtls)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {                            
                var Cancerdtls = new CancerInfo
                {                   
                    UserId = cancerdtls.UserId,
                    CancertypeId = cancerdtls.CancertypeId,
                    CancerNameId = cancerdtls.CancerNameId,
                    StageId = cancerdtls.StageId,
                    GradeId = cancerdtls.GradeId,
                    IsFirstTime = cancerdtls.IsFirstTime,
                    IsRelapsed = cancerdtls.IsRelapsed,
                    IsTreatmentOngoing = cancerdtls.IsTreatmentOngoing,
                    IsSurgery = cancerdtls.IsSurgery,
                    IsChemo = cancerdtls.IsChemo,
                    IsRadiation = cancerdtls.IsRadiation,
                    IsTargetedTherapy = cancerdtls.IsTargetedTherapy,
                    IsPallitiveCare = cancerdtls.IsPallitiveCare,
                    IsRemission = cancerdtls.IsRemission,
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

        [HttpPost("CancerTreatement")]
        public IActionResult Post(TreatementConduct model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddUser(model);
                    return Ok(new { StatusCode = 200, Message = "Treatement Details Added Sucessfully", Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddUser(TreatementConduct treatmentConducted)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {              
                var Treatement = new TreatmentConductedAt
                {
                    UserId = treatmentConducted.UserId,
                    HospitalName= treatmentConducted.HospitalName,
                    OncologistName= treatmentConducted.OncologistName,
                    Createdon = DateTime.Now,
                };
                Context.TreatmentConductedAt.Add(Treatement);
                Context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public class TreatementConduct
        {
            public int UserId { get; set; }
            public string HospitalName { get; set; }
            public string OncologistName { get; set; }
        }
    }
}
