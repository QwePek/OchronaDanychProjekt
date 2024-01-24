using Microsoft.AspNetCore.Mvc;
using WebApp.Shared.Model;
using WebApp.Shared.Services;
using WebApp.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebApp.Authentication;
using WebApp.Autorization;
using WebApp.Shared.DTO;

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
			string authorizationHeader = Request.Headers["Authorization"];
			List<Claim> claims = ApiAuthenticationStateProvider.ParseClaimsFromJwt(authorizationHeader).ToList();

            if(!JWTUtils.IsJwtTokenValid(authorizationHeader))
                return BadRequest("JWT token is expired");

            var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);
            if (userId == null)
                return BadRequest("Invalid JWT token");

			Response<User> idUser = await _userService.GetUserAsync(int.Parse(userId.Value));
            if(!idUser.Success || idUser.Data == null)
				return BadRequest("User with ID not found");

            User user = idUser.Data;
            if(user.Role != Role.Admin)
				return Unauthorized(new { message = "Unauthorized. User is not admin" });

            var result = await _userService.GetUsersAsync();
            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpGet("Get/{ID}")]
		[Authorize]
		public async Task<ActionResult<Response<User>>> Get(int ID)
        {
            var user =  (ClaimsIdentity)User.Identity;

            var exp = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

            if (!JWTUtils.IsJwtTokenValid(long.Parse(exp.Value)))
                return BadRequest("JWT token is expired");

            var userId = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);

            if (userId == null || ID != int.Parse(userId.Value))
                return Unauthorized(new { message = "Unauthorized user ID is " + userId + " and wanted " + ID });

            var result = await _userService.GetUserAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpGet("Get")]
		public async Task<ActionResult<Response<User>>> Get()
        {
			string authorizationHeader = Request.Headers["Authorization"];
			List<Claim> claims = ApiAuthenticationStateProvider.ParseClaimsFromJwt(authorizationHeader).ToList();

            if (!JWTUtils.IsJwtTokenValid(authorizationHeader))
                return BadRequest("JWT token is expired");

            var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);
            if (userId == null)
                return Unauthorized(new { message = "Unauthorized user"});

            int ID = int.Parse(userId.Value);
            var result = await _userService.GetUserAsync(ID);
            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<User>>> Add([FromBody]User user)
        {
            string authorizationHeader = Request.Headers["Authorization"];
            List<Claim> claims = ApiAuthenticationStateProvider.ParseClaimsFromJwt(authorizationHeader).ToList();

            if (!JWTUtils.IsJwtTokenValid(authorizationHeader))
                return BadRequest("JWT token is expired");

            var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);
            if (userId == null)
                return BadRequest("Invalid JWT token");

            Response<User> idUser = await _userService.GetUserAsync(int.Parse(userId.Value));
            if (!idUser.Success || idUser.Data == null)
                return BadRequest("User with ID not found");

            User tokenUser = idUser.Data;
            if (tokenUser.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized. User is not admin" });

            var result = await _userService.AddUserAsync(user);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpDelete("Delete/{ID}")]
        public async Task<ActionResult<Response<User>>> Delete(int ID)
        {
            string authorizationHeader = Request.Headers["Authorization"];
            List<Claim> claims = ApiAuthenticationStateProvider.ParseClaimsFromJwt(authorizationHeader).ToList();

            if (!JWTUtils.IsJwtTokenValid(authorizationHeader))
                return BadRequest("JWT token is expired");

            var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);
            if (userId == null)
                return BadRequest("Invalid JWT token");

            Response<User> idUser = await _userService.GetUserAsync(int.Parse(userId.Value));
            if (!idUser.Success || idUser.Data == null)
                return BadRequest("User with ID not found");

            User user = idUser.Data;
            if (user.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized. User is not admin" });
                
            var result = await _userService.DeleteUserAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Response<User>>> Update([FromBody]User user)
        {
            string authorizationHeader = Request.Headers["Authorization"];
            List<Claim> claims = ApiAuthenticationStateProvider.ParseClaimsFromJwt(authorizationHeader).ToList();

            if (!JWTUtils.IsJwtTokenValid(authorizationHeader))
                return BadRequest("JWT token is expired");

            var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);
            if (userId == null)
                return BadRequest("Invalid JWT token");

            Response<User> idUser = await _userService.GetUserAsync(int.Parse(userId.Value));
            if (!idUser.Success || idUser.Data == null)
                return BadRequest("User with this JWTToken ID not found");

            User tokenUser = idUser.Data;
            if (int.Parse(userId.Value) != user.Id && tokenUser.Role != Role.Admin)
                return Unauthorized(new { message = "Cannot modificate not authorized other accounts" });

            var result = await _userService.UpdateUserAsync(user);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

		[HttpPost("PasswordChange")]
		public async Task<ActionResult<Response>> ChangePasswordAsync([FromBody] PasswordChangeModel model)
		{
			string authorizationHeader = Request.Headers["Authorization"];
			List<Claim> claims = ApiAuthenticationStateProvider.ParseClaimsFromJwt(authorizationHeader).ToList();

			if (!JWTUtils.IsJwtTokenValid(authorizationHeader))
				return BadRequest(new Response() { Success = false, Message = "JWT token is expired" });

			var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId);
			if (userId == null)
				return BadRequest(new Response() { Success = false, Message = "Invalid JWT token" });

			Response<User> idUser = await _userService.GetUserAsync(int.Parse(userId.Value));
			if (!idUser.Success || idUser.Data == null)
				return BadRequest(new Response() { Success = false, Message = "User with this JWTToken ID not found" });

			User tokenUser = idUser.Data;
			if (int.Parse(userId.Value) != model.Id && tokenUser.Role != Role.Admin)
				return Unauthorized(new Response(){ Success = false, Message = "Cannot modificate not authorized other accounts" });

			var result = await _userService.ChangePasswordAsync(model);
			if (result.Success)
				return Ok(result);
			else
				return BadRequest(result);
		}
	}
}
