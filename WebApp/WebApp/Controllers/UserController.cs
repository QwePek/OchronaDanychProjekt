using Microsoft.AspNetCore.Mvc;
using WebApp.Shared.Model;
using WebApp.Shared.Services;
using WebApp.Shared;
using WebApp.Shared.Login;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<Response<List<User>>>> GetAll()
        {
            var result = await _userService.GetUsersAsync();

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpGet("Get/{ID}")]
        public async Task<ActionResult<Response<User>>> Get(int ID)
        {
            var result = await _userService.GetUserAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<User>>> Add([FromBody]User user)
        {
            var result = await _userService.AddUserAsync(user);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpDelete("Delete/{ID}")]
        public async Task<ActionResult<Response<User>>> Delete(int ID)
        {
            var result = await _userService.DeleteUserAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Response<User>>> Update([FromBody]User user)
        {
            var result = await _userService.UpdateUserAsync(user);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }
    }
}
