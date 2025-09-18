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
    public class OtherDetailsController : ControllerBase
    {
        private readonly IOtherDetailsRepository _repository;
        private readonly IConfiguration _configuration;

        public OtherDetailsController(IOtherDetailsRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        private string GenerateToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("PatientDetails")]
        public IActionResult AddPatient([FromBody] PatientDetails model)
        {
            _repository.AddPatient(model, model.Category);
            return Ok(new { StatusCode = 200, Token = GenerateToken(), Message = "Patient Details Added Successfully", UserId = model.UserId });
        }

        [HttpPost("OrganizationDetails")]
        public IActionResult AddOrganization([FromBody] OrganizationDetails org, [FromBody] AreaofServiceMaster area)
        {
            _repository.AddOrganization(org, area);
            return Ok(new { StatusCode = 200, Token = GenerateToken(), Message = "Organization Details Added Successfully", UserId = org.UserId });
        }

        [HttpPost("OccupationDetails")]
        public IActionResult AddOccupation([FromBody] OccupationalDetails occ)
        {
            _repository.AddOccupation(occ);
            return Ok(new { StatusCode = 200, Token = GenerateToken(), Message = "Occupation Details Added Successfully", UserId = occ.UserId });
        }

        [HttpPost("update-details")]
        public IActionResult UpdateDetails([FromBody] int userId)
        {
            _repository.MarkRegistrationCompleted(userId);
            return Ok(new { StatusCode = 200, Token = GenerateToken(), Message = "Details updated successfully", UserId = userId });
        }

        [HttpGet("PatientDetails/{userId}")]
        public IActionResult GetPatientDetails(int userId) =>
            Ok(new { StatusCode = 200, Data = _repository.GetPatientDetails(userId) });

        [HttpGet("OrganizationDetails/{userId}")]
        public IActionResult GetOrganizationDetails(int userId) =>
            Ok(new { StatusCode = 200, Data = _repository.GetOrganizationDetails(userId), AreaOfServiceDetails = _repository.GetAreaOfServiceDetails(userId) });

        [HttpGet("OccupationalDetails/{userId}")]
        public IActionResult GetOccupationalDetails(int userId) =>
            Ok(new { StatusCode = 200, Data = _repository.GetOccupationalDetails(userId) });
    }
}
