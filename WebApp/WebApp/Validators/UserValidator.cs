using WebApp.Shared;
using WebApp.Shared.Model;

namespace WebApp.Validators
{
	public class UserValidator : IValidator<User>
	{
		public Response Validate(User user)
		{
			if (user == null)
				return new Response { Success = false, Message = "Model error" };
			if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
				return new Response { Success = false, Message = "All fields are required." };
			if (user.Id < 0)
				return new Response { Success = false, Message = "ID cannot be < 0" };

			return new Response { Success = true, Message = "Data is valid" };
		}
	}
}
