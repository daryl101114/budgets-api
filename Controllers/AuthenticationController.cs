using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace budget_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IConfiguration configuration, ILogger<AuthenticationController> logger, IAuthenticationService authenticationService)
        {
            _configuration = configuration;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(AuthenticationRequestBodyDto authenticationRequestBody)
        {
            try
            {
                // Authenticate User
               var token = await _authenticationService.AuthenticateAsync(authenticationRequestBody);
                if (token == null)
                {
                    return Unauthorized();
                }
                // Set a cookie with name "MyCookie" and value "HelloWorld"
                Response.Cookies.Append("app.at", token.Value!, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(1), // Optional: cookie expires in 1 day
                    HttpOnly = false, // Optional: prevents client-side script access
                    Secure = false,   // Optional: cookie only sent over HTTPS
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                    Domain = "localhost"
                });
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong while authenticating the user", ex);
                return StatusCode(500, "A problem occured while handling request");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserCreationDto userCreationDto)
        {
            try
            {
                var createdUser = await _authenticationService.RegisterAsync(userCreationDto);
                return Ok("User has been created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong while registering the user", ex);
                return StatusCode(500, "A problem occured while handling request");
            }
        }
    }
}
