using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using static CPositiveAPI.Controllers.OtherDetailsController;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtherDetailsController : ControllerBase
    {
        public readonly ApplicationDbContext Context;

        private IConfiguration _configuration;
        public OtherDetailsController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            Context = dbContext;
            _configuration = configuration;
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


        [HttpPost("PatientDetails")]
        public IActionResult Post(Patientdtls model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = GenerateToken();
                    AddPatient(model);
                    int userId = model.UserId;
                    return Ok(new { StatusCode = 200, token = token, Message = "Patient Details Added Sucessfully", UserId = userId, Data = model });
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
                    Age = patientdtls.Age,
                    Gender = patientdtls.Gender,
                    RelWithPatient = patientdtls.RelWithPatient,
                    Createdon = DateTime.Now,
                    Category = patientdtls.Category,
                };
                Context.PatientDetails.Add(patient);
                Context.SaveChanges();

                var updateIsRegistrationCompletedSql = String.Empty;

                if (patientdtls.Category == "Caregiver")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET CaregiverPatientDetail = 'Y'
                    WHERE UserId = @UserId AND CaregiverPatientDetail != 'Y'";
                }
                else if (patientdtls.Category == "FamilyMember")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET FamilyMemberPatientDetail = 'Y'
                    WHERE UserId = @UserId AND FamilyMemberPatientDetail != 'Y'";
                }

                // Execute the update query
                var rowsAffected = Context.Database.ExecuteSqlRaw(updateIsRegistrationCompletedSql, new[]
                {
                    new SqlParameter("@UserId", patientdtls.UserId)
                });


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
            public int Age { get; set; }
            public string Gender { get; set; }
            public string RelWithPatient { get; set; }
            public string Category { get; set; }
        }


        [HttpPost("OrganizationDetails")]
        public IActionResult OrganizationDetails(Organization model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = GenerateToken();
                    AddOrganization(model);
                    int userId = model.UserId;
                    return Ok(new { StatusCode = 200, token = token, Message = "Organization Details Added Sucessfully", UserId = userId, Data = model });
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

                var userId = org.UserId;

                var areaofservice = new AreaofServiceMaster
                {
                    UserId = userId,
                    IsFinancialSupport = organization.IsFinancialSupport,
                    IsMedicalSupport = organization.IsMedicalSupport,
                    IsLogisticSupport = organization.IsLogisticSupport,
                    IsCareGiverServices = organization.IsCareGiverServices,
                    IsMentalHealthSupport = organization.IsMentalHealthSupport,
                    IsTraining = organization.IsTraining,
                    IsAwareness = organization.IsAwareness,
                    IsScreening = organization.IsScreening,
                    IsOther = organization.IsOther,
                    IfOtherTestHere = organization.IfOtherTestHere,
                    Createdon = DateTime.Now,
                };
                Context.AreaofServiceMaster.Add(areaofservice);

                var updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET OrganizationalDetails = 'Y'
                    WHERE UserId = @UserId AND OrganizationalDetails != 'Y'";

                // Execute the update query
                var rowsAffected = Context.Database.ExecuteSqlRaw(updateIsRegistrationCompletedSql, new[]
                {
                    new SqlParameter("@UserId", organization.UserId)
                });

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
            public string IsFinancialSupport { get; set; }
            public string IsMedicalSupport { get; set; }
            public string IsLogisticSupport { get; set; }
            public string IsCareGiverServices { get; set; }
            public string IsMentalHealthSupport { get; set; }
            public string IsTraining { get; set; }
            public string IsAwareness { get; set; }
            public string IsScreening { get; set; }
            public string IsOther { get; set; }
            public string IfOtherTestHere { get; set; }
        }

        [HttpPost("OccupationDetails")]
        public IActionResult OccupationDetails(Occupation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = GenerateToken();
                    AddOccupation(model);
                    int userId = model.UserId;
                    return Ok(new { StatusCode = 200, token = token, Message = "Occupation Details Added Sucessfully", UserId = userId, Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddOccupation(Occupation occupation)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                var occup = new OccupationalDetails
                {
                    UserId = occupation.UserId,
                    Qualification = occupation.Qualification,
                    Specilization = occupation.Specilization,
                    Experties = occupation.Experties,
                    Experience = occupation.Experience,
                    Createdon = DateTime.Now,
                    Category = occupation.Category,
                };
                Context.OccupationalDetails.Add(occup);
                Context.SaveChanges();

                var updateIsRegistrationCompletedSql = String.Empty;

                if (occupation.Category == "HealthcareProfessional")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET HealthcareOccupationalDetails = 'Y'
                    WHERE UserId = @UserId AND HealthcareOccupationalDetails != 'Y'";
                }
                else if (occupation.Category == "MentalHealthProfessional")
                {
                    updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET MentalHealthOccupationalDetails = 'Y'
                    WHERE UserId = @UserId AND MentalHealthOccupationalDetails != 'Y'";
                }

                // Execute the update query
                var rowsAffected = Context.Database.ExecuteSqlRaw(updateIsRegistrationCompletedSql, new[]
                {
                    new SqlParameter("@UserId", occupation.UserId)
                });

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public class Occupation
        {
            public int UserId { get; set; }
            public string Qualification { get; set; }
            public string Specilization { get; set; }
            public string Experties { get; set; }
            public string Experience { get; set; }
            public string Category { get; set; }
        }

        [HttpPost("update-details")]
        public IActionResult UpdateDetails([FromBody] UpdateDetailsDto model)
        {
            if (model == null || model.UserId == 0)
            {
                return BadRequest("Invalid input data.");
            }
            var token = GenerateToken();
            var userid = model.UserId;
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Construct the SQL query to update the IsRegistrationCompleted table
                    var updateSql = @"
                        UPDATE IsRegistrationCompleted
                        SET RegistrationCompleted = 'Y'
                        WHERE UserId = @UserId AND RegistrationCompleted != 'Y'";

                    // Execute the update query
                    var rowsAffected = Context.Database.ExecuteSqlRaw(updateSql, new[]
                    {
                        new SqlParameter("@UserId", model.UserId)
                    });

                    // Commit the transaction
                    transaction.Commit();

                    // Return success response
                    return Ok(new { StatusCode = 200, token = token, Message = "Details updated successfully", UserId = userid, RowsAffected = rowsAffected });
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if there is an error
                    transaction.Rollback();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }


        public class UpdateDetailsDto
        {
            public int UserId { get; set; }
        }

        [HttpGet("PersonalDetails/{UserId}")]
        public IActionResult GetPersonalDetails(int UserId)
        {
            // Fetch the PersonalDetails of UserId from the database

            var personalDetails = Context.PersonalDetails.Where(d => d.UserId == UserId).ToList();
            int CountryId = 0, StateId = 0, DistrictId = 0;

            if (personalDetails.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "Personal Details Not Found" });
            }
            else
            {
                CountryId = personalDetails[0].CountryId;
                StateId = personalDetails[0].StateId;
                DistrictId = personalDetails[0].DistrictId;
            }

            var CountryName = Context.CountryMaster.Where(d => d.CountryId == CountryId).Select(T => T.CountryName).ToList();
            var StateName = Context.StateMaster.Where(d => d.stateid == StateId).Select(T => T.statename).ToList();
            var DistrictName = Context.DistrictMaster.Where(d => d.districtid == DistrictId).Select(T => T.districtname).ToList();

            return Ok(new { StatusCode = 200, Message = "Success", Data = personalDetails, CountryName, StateName, DistrictName });
        }
        [HttpGet("CancerInfo/{UserId}")]
        public IActionResult GetCancerInfoDetails(int UserId)
        {
            // Fetch the CancerInfoDetails of UserId from the database

            var cancerInfoDetails = Context.CancerInfo.Where(d => d.UserId == UserId).ToList();
            int CancerTypeId = 0, StageId = 0, GradeId = 0;

            if (cancerInfoDetails.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "Cancer Info Details Not Found" });
            }
            else
            {
                CancerTypeId = cancerInfoDetails[0].CancertypeId;
                StageId = cancerInfoDetails[0].StageId;
                GradeId = cancerInfoDetails[0].GradeId;
            }

            var CancerType = Context.CancerTypesMaster.Where(d => d.CancerTypeId == CancerTypeId).Select(T => T.CancerType).ToList();
            var Stage = Context.StageMaster.Where(d => d.stageId == StageId).Select(T => T.stagename).ToList();
            var Grade = Context.GradeMaster.Where(d => d.GradeId == GradeId).Select(T => T.GradeName).ToList();

            return Ok(new { StatusCode = 200, Message = "Success", Data = cancerInfoDetails, CancerType, Stage, Grade });
        }
        [HttpGet("TreatmentConductedAt/{UserId}")]
        public IActionResult GetTreatmentConductedAtDetails(int UserId)
        {
            // Fetch the TreatmentConductedAtDetails of UserId from the database
            var treatmentConductedAtDetails = Context.TreatmentConductedAt.Where(d => d.UserId == UserId).ToList();

            if (treatmentConductedAtDetails.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "Treatment Conducted At Details Not Found" });
            }
            return Ok(new { StatusCode = 200, Message = "Success", Data = treatmentConductedAtDetails });
        }
        [HttpGet("PatientDetails/{UserId}")]
        public IActionResult GetPatientDetails(int UserId)
        {
            // Fetch the PatientDetails of UserId from the database
            var patientDetails = Context.PatientDetails.Where(d => d.UserId == UserId).ToList();

            if (patientDetails.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "Patient Details Not Found" });
            }
            return Ok(new { StatusCode = 200, Message = "Success", Data = patientDetails });
        }
        [HttpGet("OrganizationDetails/{UserId}")]
        public IActionResult GetOrganizationDetails(int UserId)
        {
            // Fetch the OrganizationDetails of UserId from the database

            var organizationDetails = Context.OrganizationDetails.Where(d => d.UserId == UserId).ToList();

            if (organizationDetails.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "Organization Details Not Found" });
            }
            return Ok(new { StatusCode = 200, Message = "Success", Data = organizationDetails });
        }
        [HttpGet("OccupationalDetails/{UserId}")]
        public IActionResult GetOccupationalDetails(int UserId)
        {
            // Fetch the OccupationalDetails of UserId from the database

            var occupationalDetails = Context.OccupationalDetails.Where(d => d.UserId == UserId).ToList();

            if (occupationalDetails.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "Occupational Details Not Found" });
            }
            return Ok(new { StatusCode = 200, Message = "Success", Data = occupationalDetails });
        }

        [HttpGet("ProfileImage/{ImageUrl}")]
        public IActionResult ProfileImage(String ImageUrl)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images/" + ImageUrl);
            var Provider = new FileExtensionContentTypeProvider();

            if (!Provider.TryGetContentType(FilePath, out var contentType))
            {
                contentType = "application/ocet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(FilePath);
            return File(bytes, contentType, Path.GetFileName(FilePath));
        }
    }
}
