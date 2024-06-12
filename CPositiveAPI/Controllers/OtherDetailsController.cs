using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtherDetailsController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        public OtherDetailsController(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        [HttpPost("PatientDetails")]
        public IActionResult Post(Patientdtls model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddPatient(model);
                    return Ok(new { StatusCode = 200, Message = "Patient Details Added Sucessfully", Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddPatient(Patientdtls patientdtls)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                var patient = new PatientDetails
                {
                    UserId = patientdtls.UserId,
                    PatientName = patientdtls.PatientName,
                    RelWithPatient = patientdtls.RelWithPatient,
                    Createdon = DateTime.Now,
                };
                Context.PatientDetails.Add(patient);
                Context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public class Patientdtls
        {
            public int UserId { get; set; }
            public string PatientName { get; set; }
            public string RelWithPatient { get; set; }
        }

        [HttpPost("OrganizationDetails")]
        public IActionResult OrganizationDetls(Organization model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddOrganization(model);
                    return Ok(new { StatusCode = 200, Message = "Organization Details Added Sucessfully", Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddOrganization(Organization organization)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                var org = new OrganizationDetails
                {
                    UserId = organization.UserId,
                    OrgName = organization.OrgName,
                    OrgWebsite = organization.OrgWebsite,
                    OrgEmail = organization.OrgEmail,
                    OrgMobileNumber = organization.OrgMobileNumber,
                    OrgAddress = organization.OrgAddress,
                    Createdon = DateTime.Now,
                };
                Context.OrganizationDetails.Add(org);
                Context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public class Organization
        {
            public int UserId { get; set; }
            public string OrgName { get; set; }
            public string OrgWebsite { get; set; }
            public string OrgEmail { get; set; }
            public string OrgMobileNumber { get; set; }
            public string OrgAddress { get; set; }
        }
    }
}
