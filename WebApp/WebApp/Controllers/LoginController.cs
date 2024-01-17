using Microsoft.AspNetCore.Mvc;
using WebApp.Shared.Login;
using WebApp.Shared;
using WebApp.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection.Metadata.Ecma335;
using WebApp.Shared.Model;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Response<bool>>> Register([FromBody] RegisterModel model)
        {
            var result = await _loginService.RegisterAsync(model);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, $"Internal server error {result.Message}");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            await Task.Delay(1000);
            if (!ModelState.IsValid)
            {
                return LocalRedirect("/Login");
            }

            LoginModel model = new LoginModel();
            model.Email = email;
            model.Password = password;
            var result = await _loginService.LoginAsync(model);

            if (result.Success)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Data);
                return LocalRedirect("/Profile");
            }
            else
                return LocalRedirect("/Login");//StatusCode((int)System.Net.HttpStatusCode.InternalServerError, $"Internal server error {result.Message}");
        }

        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await Task.Delay(1000);
            if (!ModelState.IsValid)
            {
                return LocalRedirect("/Profile");
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return LocalRedirect("/Login");
        }
    }
}
