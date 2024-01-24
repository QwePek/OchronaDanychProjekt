using WebApp.Shared.DTO;
using WebApp.Shared.Model;

namespace WebApp.Shared.Services
{
	public interface IUserService
	{
		Task<Response<List<User>>> GetUsersAsync();
		Task<Response> DeleteUserAsync(int ID);
		Task<Response<User>> GetUserAsync(int ID);
		Task<Response> AddUserAsync(User user);
		Task<Response> UpdateUserAsync(User user);
		Task<Response> ChangePasswordAsync(PasswordChangeModel model);
	}
}
