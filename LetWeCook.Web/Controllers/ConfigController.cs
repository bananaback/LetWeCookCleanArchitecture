using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("google-api-key")]
        public IActionResult GetGoogleApiKey()
        {
            var apiKey = _configuration["GoogleGenerativeAI:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest(new { error = "Google Generative AI API key not configured" });
            }

            return Ok(new { apiKey = apiKey });
        }
    }
}