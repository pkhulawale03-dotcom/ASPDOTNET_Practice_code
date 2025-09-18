using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPatientController : ControllerBase
    {
        private readonly ICPatientRepository _repository;
        private readonly IConfiguration _configuration;

        public CPatientController(ICPatientRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpGet("GetCategory")]
        public IActionResult GetCategory()
        {
            var categories = _repository.GetCategories();
            if (!categories.Any())
                return NotFound(new { StatusCode = 404, Message = "Category Not Found" });

            return Ok(new { StatusCode = 200, Message = "Success", Data = categories });
        }

        [HttpGet("check-username")]
        public IActionResult CheckUsernameExists([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(new { StatusCode = 400, Message = "Username is required" });

            bool exists = _repository.UsernameExists(username);
            return Ok(new { StatusCode = 200, Message = exists ? "Username exists" : "Username available", Data = username });
        }

        [HttpGet("check-mobileno")]
        public IActionResult CheckMobilenoExists([FromQuery] string mobileno)
        {
            if (string.IsNullOrEmpty(mobileno))
                return BadRequest(new { StatusCode = 400, Message = "Mobile number is required" });

            bool exists = _repository.MobileExists(mobileno);
            return Ok(new { StatusCode = 200, Message = exists ? "Mobile already exists" : "Available", Data = mobileno });
        }

        [HttpGet("check-EmailId")]
        public IActionResult CheckEmailIdExists([FromQuery] string emailId)
        {
            if (string.IsNullOrEmpty(emailId))
                return BadRequest(new { StatusCode = 400, Message = "Email is required" });

            bool exists = _repository.EmailExists(emailId);
            return Ok(new { StatusCode = 200, Message = exists ? "Email already exists" : "Available", Data = emailId });
        }

        private string GenerateToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                null, expires: DateTime.Now.AddMinutes(5), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("AddUsers")]
        public IActionResult AddUsers([FromBody] Users model, [FromQuery] UserCategoryLink category, [FromQuery] IsRegistrationCompleted registration)
        {
            var token = GenerateToken();
            var userId = _repository.AddUser(model, category, registration);
            return Ok(new { StatusCode = 200, Token = token, Message = "User Added Successfully", UserId = userId });
        }

        [HttpPost("add-personal-details")]
        public async Task<IActionResult> AddPersonalDetails([FromBody] PersonalDetls model)
        {
            var token = GenerateToken();
            var savedDetails = await _repository.GetPersonalDetlsAsync(model);
            return Ok(new { StatusCode = 200, Token = token, Message = "Personal Details Added Successfully", Data = savedDetails });
        }

        [HttpGet("countries")]
        public IActionResult GetCountries() =>
            Ok(new { StatusCode = 200, Data = _repository.GetCountries() });

        [HttpGet("states/{countryId}")]
        public IActionResult GetStates(int countryId) =>
            Ok(new { StatusCode = 200, Data = _repository.GetStates(countryId) });

        [HttpGet("districts/{stateId}")]
        public IActionResult GetDistricts(int stateId) =>
            Ok(new { StatusCode = 200, Data = _repository.GetDistricts(stateId) });
    }
}
