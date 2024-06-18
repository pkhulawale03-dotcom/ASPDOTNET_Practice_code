using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPatientController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        //private readonly UserManager<ApplicationUser> _userManager; // Add UserManager
        //public CPatientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        //{
        //    _context = context;
        //    _userManager = userManager;
        //}
        private IConfiguration _configuration;
        //public EncryptionDecryption enDn = new EncryptionDecryption();
        public CPatientController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;

            Context = dbContext;            
        }

       
        [HttpGet]
        [Route("GetCategory")]
        public IActionResult GetCategory()
        {
            try
            {
                var Users = Context.UserCategoryMaster.ToList();
                if (Users.Count == 0)
                {
                    return NotFound(new { StatusCode = 404, Message = "Category Not Found" });
                }
                return Ok(new { StatusCode = 200, Message = "Success", Data = Users });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
        [HttpGet("check-username")]
        public IActionResult CheckUsernameExists([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { StatusCode = 400, Message = "Username is required" });
            }

            bool exists = Context.Users.Any(u => u.Username == username);
            if (exists)
            {
                return Ok(new { StatusCode = 200, Message = "Username exists", Data=username });
            }
            return Ok(new { StatusCode = 200, Message = "Username is available", Data = username });
        }

        
        [HttpGet("check-mobileno")]
        public IActionResult CheckMobilenoExists([FromQuery] string mobileno)
        {
            if (string.IsNullOrEmpty(mobileno))
            {
                return BadRequest(new { StatusCode = 400, Message = "Mobile number is required" });
            }

            bool exists = Context.Users.Any(u => u.Mobileno == mobileno);
            if (exists)
            {
                return Ok(new { StatusCode = 200, Message = "Mobile number exists", Data = mobileno });
            }
            return Ok(new { StatusCode = 200, Message = "Mobile number is available", Data = mobileno });
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

        [HttpPost("AddUsers")]
        public IActionResult AddUsers([FromBody] CreateUserDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddUser(model);
                    var token = GenerateToken();
                    var userId = AddUser(model); // Get the generated UserId
                    return Ok(new { StatusCode = 200, token = token, Message = "User Added Successfully", UserId = userId, Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private int AddUser(CreateUserDto newUser)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                // Create and add the user login details
                var userLogin = new Users
                {
                    UserId = newUser.userId,
                    Username = newUser.Username,
                    Password = newUser.Password,
                    ConfirmPassword = newUser.ConfirmPassword,
                    EmailId = newUser.EmailId,
                    Mobileno = newUser.Mobileno,
                    Createdon = DateTime.Now,
                };
                Context.Users.Add(userLogin);
                Context.SaveChanges(); // Save to generate the UserId
               
                // Retrieve the newly generated UserId
                var userId = userLogin.UserId;
                // Create and add the user category link
                var userCategory = new UserCategoryLink
                {
                    UserId = userId,
                    CPositive = newUser.IsCPositive,
                    Caregiver = newUser.IsCaregiver,
                    FamilyMember = newUser.IsFamilyMember,
                    Volunteer = newUser.IsVolunteer,
                    HealthcareProfessional = newUser.IsHealthcareProfessional,
                    MentalHealthProfessional = newUser.IsMentalHealthProfessional
                };
                Context.UserCategoryLinking.Add(userCategory);

                // Create and add the registration completion record
                var isRegistrationCompleted = new IsRegistrationCompleted
                {
                    UserId = userId,
                    Createdon = DateTime.Now,
                };
                Context.IsRegistrationCompleted.Add(isRegistrationCompleted);

                // Save all changes in one go
                Context.SaveChanges();

                // Commit the transaction
                transaction.Commit();
                return userId; // Return the generated UserId
            }
            catch (Exception)
            {
                // Rollback the transaction in case of an error
                transaction.Rollback();
                throw;
            }
        }


        public class CreateUserDto
        {
            public int userId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
            public string EmailId { get; set; }
            public string Mobileno { get; set; }

            // Properties for category selections
            public string IsCPositive { get; set; }
            public string IsCaregiver { get; set; }
            public string IsFamilyMember { get; set; }
            public string IsVolunteer { get; set; }
            public string IsHealthcareProfessional { get; set; }
            public string IsMentalHealthProfessional { get; set; }          
        }

        public class CreatePersonalDetlsDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public int CountryId { get; set; }
            public int StateId { get; set; }
            public int DistrictId { get; set; }
            public string Address { get; set; }
            public string Pincode { get; set; }
            public string Age { get; set; }
            public string Gender { get; set; }
            public string HighestQualification { get; set; }
            public string Occupation { get; set; }
        }

        [Authorize]
        [HttpPost("add-personal-details")]
        public IActionResult AddPersonalDetails([FromBody] CreatePersonalDetlsDto model)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Create a new record for the PersonalDetls table
                        var personalDetls = new PersonalDetls
                        {
                            UserId = model.UserId,
                            Name = model.Name,
                            CountryId = model.CountryId,
                            StateId = model.StateId,
                            DistrictId = model.DistrictId,
                            Address = model.Address,
                            Pincode = model.Pincode,
                            Age = model.Age,
                            Gender = model.Gender,
                            HighestQualification = model.HighestQualification,
                            Occupation = model.Occupation,
                            Createdon = DateTime.Now
                        };

                        // Insert the new record into PersonalDetls
                        Context.PersonalDetails.Add(personalDetls);
                        Context.SaveChanges();

                        // Construct the SQL query to update the IsRegistrationCompleted table
                        var updateIsRegistrationCompletedSql = @"
                    UPDATE IsRegistrationCompleted
                    SET Personaldetails = 'Y'
                    WHERE UserId = @UserId AND Personaldetails IS NULL";

                        // Execute the update query
                        var rowsAffected = Context.Database.ExecuteSqlRaw(updateIsRegistrationCompletedSql, new[]
                        {
                    new SqlParameter("@UserId", model.UserId)
                });

                        // Commit the transaction
                        transaction.Commit();

                        // Return success response
                        return Ok(new { StatusCode = 200, Message = "Personal details added successfully", Data = personalDetls });
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if there is an error
                        transaction.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }

            return BadRequest(ModelState);
        }




        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var countries = Context.CountryMaster.ToList();
            return Ok(new { StatusCode = 200, Data = countries });
        }

       
        [HttpGet("states/{countryId}")]
        public IActionResult GetStates(int countryId)
        {
            // Fetch the states from the database
            var states = Context.StateMaster.Where(s => s.CountryId == countryId).ToList();

            // Use the name of the action method to generate the data property name
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var dataPropertyName = $"{actionName}Data";

            // Create the response object dynamically
            var responseObject = new ExpandoObject() as IDictionary<string, Object>;
            responseObject.Add("StatusCode", 200);
            responseObject.Add(dataPropertyName, states); // Use the generated name

            return Ok(responseObject);
        }



        [HttpGet("districts/{stateId}")]
        public IActionResult GetDistricts(int stateId)
        {
            // Fetch the districts from the database
            var districts = Context.DistrictMaster.Where(d => d.stateid == stateId).ToList();

            // Use the name of the action method to generate the data property name
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var dataPropertyName = $"{actionName}Data";

            // Create the response object dynamically
            var responseObject = new ExpandoObject() as IDictionary<string, Object>;
            responseObject.Add("StatusCode", 200);
            responseObject.Add(dataPropertyName, districts); // Use the generated name

            return Ok(responseObject);
        }
    }
}
