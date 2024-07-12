using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using static CPositiveAPI.Controllers.CPatientController;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancerDetailsController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        private IConfiguration _configuration;
        public CancerDetailsController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            Context = dbContext;
            _configuration = configuration;
        }


        [HttpGet("CancerName")]
        public IActionResult GetCancerName()
        {
            var cancername = Context.CancerNameMaster.ToList();

            // Use the name of the action method to generate the data property name
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var dataPropertyName = $"{actionName}Data";

            // Create the response object dynamically
            var responseObject = new ExpandoObject() as IDictionary<string, Object>;
            responseObject.Add("StatusCode", 200);
            responseObject.Add(dataPropertyName, cancername); // Use the generated name

            return Ok(responseObject);

        }


        [HttpGet("CancerType")]
        public IActionResult GetCancerType()
        {
            var cancertype = Context.CancerTypesMaster.ToList();
            // Use the name of the action method to generate the data property name
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var dataPropertyName = $"{actionName}Data";

            // Create the response object dynamically
            var responseObject = new ExpandoObject() as IDictionary<string, Object>;
            responseObject.Add("StatusCode", 200);
            responseObject.Add(dataPropertyName, cancertype); // Use the generated name

            return Ok(responseObject);
        }


        [HttpGet("CancerGrades")]
        public IActionResult GetCancerGrades()
        {
            var cancergrade = Context.GradeMaster.ToList();
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var dataPropertyName = $"{actionName}Data";

            // Create the response object dynamically
            var responseObject = new ExpandoObject() as IDictionary<string, Object>;
            responseObject.Add("StatusCode", 200);
            responseObject.Add(dataPropertyName, cancergrade); // Use the generated name

            return Ok(responseObject);
        }


        [HttpGet("CancerStages")]
        public IActionResult GetCancerStages()
        {
            var cancerstage = Context.StageMaster.ToList();
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var dataPropertyName = $"{actionName}Data";

            // Create the response object dynamically
            var responseObject = new ExpandoObject() as IDictionary<string, Object>;
            responseObject.Add("StatusCode", 200);
            responseObject.Add(dataPropertyName, cancerstage); // Use the generated name

            return Ok(responseObject);
        }


        [HttpPost("CancerDetails")]
        public IActionResult AddCancerDetails([FromBody] CreateCancerDetails model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = GenerateToken();
                    AddDetails(model);
                    int userId = model.UserId;
                    return Ok(new { StatusCode = 200, token = token, Message = "Cancer Details Added Successfully", UserId = userId, Data = model });
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
                    Createdon = DateTime.Now,
                    Category = cancerdtls.Category,
                };

                Context.CancerInfo.Add(Cancerdtls);
                Context.SaveChanges();

                var updateIsRegistrationCompletedSql = String.Empty;
                if (cancerdtls.Category == "Cpatient")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET CpatientCancerInfo = 'Y'
                    WHERE UserId = @UserId";
                }
                else if (cancerdtls.Category == "Caregiver")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET CaregiverCancerInfo = 'Y'
                    WHERE UserId = @UserId";
                }
                else if (cancerdtls.Category == "FamilyMember")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET FamilyMemberCancerInfo = 'Y'
                    WHERE UserId = @UserId";
                }

                // Execute the update query
                var rowsAffected = Context.Database.ExecuteSqlRaw(updateIsRegistrationCompletedSql, new[]
                {
                    new SqlParameter("@UserId", cancerdtls.UserId)
                });

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
            public string Category { get; set; }
        }

        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("CancerTreatement")]
        public IActionResult Post([FromBody] TreatmentRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = GenerateToken();
                    foreach (var treatment in model.Treatments)
                    {
                        AddTreatment(new TreatementConduct
                        {
                            UserId = model.UserId,
                            HospitalName = treatment.HospitalName,
                            OncologistName = treatment.OncologistName,
                            Category = treatment.Category,
                        });
                    }
                    return Ok(new { StatusCode = 200, token = token, Message = "Treatment Details Added Successfully" });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddTreatment(TreatementConduct treatmentConducted)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                var Treatement = new TreatmentConductedAt
                {
                    UserId = treatmentConducted.UserId,
                    HospitalName = treatmentConducted.HospitalName,
                    OncologistName = treatmentConducted.OncologistName,
                    Createdon = DateTime.Now,
                    Category = treatmentConducted.Category,
                };
                Context.TreatmentConductedAt.Add(Treatement);
                Context.SaveChanges();

                var updateIsRegistrationCompletedSql = String.Empty;

                if (treatmentConducted.Category == "Cpatient")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET CpatientTreatmentConducted = 'Y'
                    WHERE UserId = @UserId AND CpatientTreatmentConducted != 'Y'";
                }
                else if (treatmentConducted.Category == "Caregiver")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET CaregiverTreatmentConducted = 'Y'
                    WHERE UserId = @UserId AND CaregiverTreatmentConducted != 'Y'";
                }
                else if (treatmentConducted.Category == "FamilyMember")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET FamilyMemberTreatmentConducted = 'Y'
                    WHERE UserId = @UserId AND FamilyMemberTreatmentConducted != 'Y'";
                }


                var rowsAffected = Context.Database.ExecuteSqlRaw(updateIsRegistrationCompletedSql, new[]
                {
            new SqlParameter("@UserId", treatmentConducted.UserId)
        });

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public class TreatmentRequest
        {
            public int UserId { get; set; }
            public List<TreatementConduct> Treatments { get; set; }
        }

        public class TreatementConduct
        {
            public int UserId { get; set; }
            public string HospitalName { get; set; }
            public string OncologistName { get; set; }
            public string Category { get; set; }
        }

    }
}
