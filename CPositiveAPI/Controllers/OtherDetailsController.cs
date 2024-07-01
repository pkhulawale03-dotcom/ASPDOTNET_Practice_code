using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
                    return Ok(new { StatusCode = 200,token = token, Message = "Patient Details Added Sucessfully",UserId = userId, Data = model });
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
                };
                Context.PatientDetails.Add(patient);
                Context.SaveChanges();

                var updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET PatientDetails = 'Y'
                    WHERE UserId = @UserId AND PatientDetails != 'Y'";

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
            public int Age {  get; set; }
            public string Gender {  get; set; }
            public string RelWithPatient { get; set; }
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
                    return Ok(new { StatusCode = 200,token = token, Message = "Organization Details Added Sucessfully",UserId = userId, Data = model });
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
                    return Ok(new { StatusCode = 200,token = token, Message = "Occupation Details Added Sucessfully",UserId = userId, Data = model });
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
                };
                Context.OccupationalDetails.Add(occup);
                Context.SaveChanges();

                var updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET OccupationalDetails = 'Y'
                    WHERE UserId = @UserId AND OccupationalDetails != 'Y'";

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
            public string Qualification {  get; set; }
            public string Specilization { get; set; }
            public string Experties { get; set; }
            public string Experience { get; set; }
        }

        [HttpPost("update-details")]
        public IActionResult UpdateDetails([FromBody] UpdateDetailsDto model)
        {
            if (model == null || model.UserId == 0)
            {
                return BadRequest("Invalid input data.");
            }
            var token = GenerateToken();
            var userid=model.UserId;
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
                    return Ok(new { StatusCode = 200,token = token, Message = "Details updated successfully",UserId = userid, RowsAffected = rowsAffected });
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
  }
}
