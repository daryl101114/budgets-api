using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<String>> Authenticate(AuthenticationRequestBodyDto authenticationRequestBody)
        {
            try
            {
                // Authenticate User
                var token = await _authenticationService.AuthenticateAsync(authenticationRequestBody);
                if (token == null)
                {
                    return Unauthorized();
                }
                return Ok(token);
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
