using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Login;
using WebApp.Shared.Model;

namespace WebApp.Shared.Services
{
	public interface IUserService
	{
		Task<Response<List<User>>> GetUsersAsync();
		Task<Response<bool>> DeleteUserAsync(int ID);
		Task<Response<User>> GetUserAsync(int ID);
		Task<Response<User>> AddUserAsync(User user);
		Task<Response<User>> UpdateUserAsync(User user);
	}
}
