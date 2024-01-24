using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using WebApp.Shared;
using WebApp.Shared.DTO;

namespace WebApp.Validators
{
	public class RegisterModelValidator : IValidator<RegisterModel>
	{
		public Response Validate(RegisterModel user)
		{
			if(user == null)
				return new Response { Success = false, Message = "Model error" };
			if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
				return new Response { Success = false, Message = "All fields are required." };

			Match m = Regex.Match(user.Email, @"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$");
			if (!m.Success)
				return new Response { Success = false, Message = "Email is not valid" };

			if (user.Password.Length < 8)
				return new Response { Success = false, Message = "Password must be at least 8 characters long." };
			if (user.Password.Length > 32)
				return new Response { Success = false, Message = "Password must be at at most 32 characters long." };

			if (!user.Password.Any(char.IsLower))
				return new Response { Success = false, Message = "Password must contain at least 1 lowercase letter" };
			if (!user.Password.Any(char.IsUpper))
				return new Response { Success = false, Message = "Password must contain at least 1 uppercase letter" };
			if (!user.Password.Any(char.IsDigit))
				return new Response { Success = false, Message = "Password must contain at least 1 digit" };
			if (!user.Password.Any(c => !char.IsLetterOrDigit(c)))
				return new Response { Success = false, Message = "Password must contain at least 1 non-alpha-numeric character" };

			return new Response { Success = true, Message = "Data is valid" };
		}
	}
}
