using Microsoft.AspNetCore.Mvc;
using WebApp.Shared;
using WebApp.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using WebApp.Shared.DTO;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
        {
            _authService = authService;
		}

		[HttpPost("Register")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<Response<bool>>> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);

            if (!result.Success)
				return StatusCode(500, $"Internal server error {result.Message}");

			return Ok(result);
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Authenticate([FromBody] LoginModel model)
        {
            var result = await _authService.Authenticate(model);

            if (!result.Success)
                if (result.Message == "LimitLogowan")
                    return BadRequest("Przekroczono limit nieudanych logowan");
                else
                    return BadRequest(result.Message);

            return Ok(result.Data.Token);
		}

		[HttpPost("Login")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            await Task.Delay(1000);
            if (!ModelState.IsValid)
                return BadRequest("Model is not valid");
            
            var result = await _authService.LoginAsync(model);

            if (!result.Success)
                if (result.Message == "LimitLogowan")
                    return BadRequest("Przekroczono limit nieudanych logowan");
                else if (result.Message == "SecondFactorError")
                    return Ok("Provide second factor code");
                else
                    return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await Task.Delay(1000);
            if (!ModelState.IsValid)
                return LocalRedirect("/Profile");

            await _authService.Logout();

			//await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            return LocalRedirect("/Login");
        }
	}
}
