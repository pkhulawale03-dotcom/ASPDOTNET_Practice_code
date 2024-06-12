using CPositiveAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private IConfiguration _configuration;
        public readonly ApplicationDbContext Context;
        public AuthenticateController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            Context = dbContext;
        }
    }
}
