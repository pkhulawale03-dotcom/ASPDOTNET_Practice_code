using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using CPositiveAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancerDetailsController : ControllerBase
    {
        private readonly ICancerRepository _cancerRepo;
        private readonly ITreatmentRepository _treatmentRepo;
        private readonly IConfiguration _configuration;
        public CancerDetailsController(
            ICancerRepository cancerRepo,
            ITreatmentRepository treatmentRepo,
            IConfiguration configuration)
        {
            _cancerRepo = cancerRepo;
            _treatmentRepo = treatmentRepo;
            _configuration = configuration;
        }

        [HttpGet("CancerName")]
        public IActionResult GetCancerName()
        {
            var cancername = _cancerRepo.GetCancerNames();
            return Ok(new { StatusCode = 200, Data = cancername });
        }

        [HttpGet("CancerType")]
        public IActionResult GetCancerType()
        {
            var cancertype = _cancerRepo.GetCancerType();
            return Ok(new { StatusCode = 200, Data = cancertype });
        }

        [HttpGet("CancerGrades")] 
        public IActionResult GetCancerGrades()
        {
            var cancergrade = _cancerRepo.GetGrades();
            return Ok(new { StatusCode = 200, Data = cancergrade });
        }

        [HttpGet("CancerStages")]
        public IActionResult GetCancerStages()
        {
            var cancerstage = _cancerRepo.GetStages();
            return Ok(new { StatusCode = 200, Data = cancerstage });
        }

        [HttpPost("CancerDetails")]
        public IActionResult AddCancerDetails([FromBody] CancerInfo model)
        {
            if (ModelState.IsValid)
            {
                var token = GenerateToken();
                _cancerRepo.AddCancerdetails(model, model.Category);
                return Ok(new { StatusCode = 200, token = token, Message = "Cancer Details Added Successfully", Data = model });
            }
            return BadRequest(ModelState);
        }

        [HttpPost("CancerTreatment")]
        public IActionResult AddTreatment([FromBody] TreatmentConductedAt model)
        {
            if (ModelState.IsValid)
            {
                var token = GenerateToken();
                _treatmentRepo.AddTreatment(model, model.Category);
                return Ok(new { StatusCode = 200, token = token, Message = "Treatment Details Added Successfully", Data = model });
            }
            return BadRequest(ModelState);
        }

        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
