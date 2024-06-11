using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPatientController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
       

        public CPatientController(ApplicationDbContext dbContext)
        {
            Context=dbContext;            
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
                return Ok(new { StatusCode = 200, Message = "Username exists" });
            }
            return Ok(new { StatusCode = 200, Message = "Username is available" });
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
                return Ok(new { StatusCode = 200, Message = "Mobile number exists" });
            }
            return Ok(new { StatusCode = 200, Message = "Mobile number is available" });
        }

        [HttpPost("AddUsers")]
        public IActionResult AddUsers([FromBody] CreateUserDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddUser(model);
                    return Ok(new { StatusCode = 200, Message = "User Added Successfully", Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddUser(CreateUserDto newUser)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {                          
                var UserLogin = new Users
                {                  
                    Username = newUser.Username,
                    Password = newUser.Password,
                    ConfirmPassword = newUser.ConfirmPassword,
                    EmailId = newUser.EmailId,
                    Mobileno = newUser.Mobileno,
                };
                Context.Users.Add(UserLogin);
                Context.SaveChanges();

                var userId = UserLogin.UserId;

                var usercategory = new UserCategoryLink
                {
                    UserId = userId,
                    CPositive = newUser.IsCPositive,
                    Caregiver = newUser.IsCaregiver,
                    FamilyMember = newUser.IsFamilyMember,
                    Volunteer = newUser.IsVolunteer,
                    HealthcareProfessional = newUser.IsHealthcareProfessional,
                    MentalHealthProfessional = newUser.IsMentalHealthProfessional
                };

                Context.UserCategoryLinking.Add(usercategory);
                Context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public class CreateUserDto
        {
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

    }
}
