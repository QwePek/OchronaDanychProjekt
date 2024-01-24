using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using WebApp.Shared;
using WebApp.Shared.DTO;
using WebApp.Shared.Model;

namespace WebApp.Validators
{
	public class PasswordChangeModelValidator : IValidator<PasswordChangeModel>
	{
		public Response Validate(PasswordChangeModel model)
		{
			if (model == null)
				return new Response { Success = false, Message = "Model error" };

			if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewSecondFactor))
				return new Response { Success = false, Message = "All fields are required." };

			if (model.NewPassword.Length < 8)
				return new Response { Success = false, Message = "New password must be at least 8 characters long." };
			if (model.NewPassword.Length > 32)
				return new Response { Success = false, Message = "New password must be at at most 32 characters long." };
			
			if (model.NewSecondFactor.Length != 5)
				return new Response { Success = false, Message = "Must fill all second factor fields." };

			if (!model.NewPassword.Any(char.IsLower))
				return new Response { Success = false, Message = "New password must contain at least 1 lowercase letter" };
			if (!model.NewPassword.Any(char.IsUpper))
				return new Response { Success = false, Message = "New password must contain at least 1 uppercase letter" };
			if (!model.NewPassword.Any(char.IsDigit))
				return new Response { Success = false, Message = "New password must contain at least 1 digit" };
			if (!model.NewPassword.Any(c => !char.IsLetterOrDigit(c)))
				return new Response { Success = false, Message = "New password must contain at least 1 non-alpha-numeric character" };

			return new Response { Success = true, Message = "Data is valid" };
		}
	}
}
