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
        [Route("Get")]
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

        [HttpPost]
        public IActionResult Post(Users model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddUser(model);
                    return Ok(new { StatusCode = 200, Message = "User Added Sucessfully", Data = model });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        private void AddUser(Users newUser)
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

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
