using System.Text.RegularExpressions;
using WebApp.Shared;
using WebApp.Shared.DTO;

namespace WebApp.Validators
{
	public class LoginModelValidator : IValidator<LoginModel>
	{
		public Response Validate(LoginModel user)
		{
			if (user == null)
				return new Response { Success = false, Message = "Model error" };
			if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
				return new Response { Success = false, Message = "All fields are required." };

			Match m = Regex.Match(user.Email, @"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$");
			if (!m.Success)
				return new Response { Success = false, Message = "Email is not valid" };

            //Second factor
            if (string.IsNullOrEmpty(user.SecondFactorPassword))
                return new Response { Success = false, Message = "SecondFactorError" };

            if (user.SecondFactorPassword.Length != 5)
                return new Response { Success = false, Message = "Second Factor password length is invalid" };

            return new Response { Success = true, Message = "Data is valid" };
		}
	}
}
