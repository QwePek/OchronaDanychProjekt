using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using WebApp.Data;
using WebApp.Shared;
using WebApp.Shared.Login;
using WebApp.Shared.Model;
using WebApp.Shared.Services;

namespace WebApp.Services
{
	public class UserService : IUserService
	{
		private readonly DataContext _dataContext;

		public UserService(DataContext context)
		{
			_dataContext = context;
		}

		public async Task<Response<List<User>>> GetUsersAsync()
		{
			var users = await _dataContext.Users.ToListAsync();
			try
			{
				var response = new Response<List<User>>()
				{
					Data = users,
					Message = "Ok",
					Success = true
				};

				return response;
			}
			catch (Exception)
			{
				return new Response<List<User>>()
				{
					Data = null,
					Message = "Problem with dataseeder library",
					Success = false
				};
			}
		}

		public async Task<Response<bool>> DeleteUserAsync(int ID)
		{
			var res = new Response<bool>();
			try
			{
				if (ID < 0)
				{
					res.Data = false;
					res.Success = false;
					res.Message = "User couldn't be deleted - ID cannot be < 0";
					return res;
				}

				var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(b => b.Id == ID);
				if (sameIDUser != null)
				{
					_dataContext.Users.Remove(sameIDUser);
					await _dataContext.SaveChangesAsync();
					res.Success = true;
					res.Message = "User deleted successfully";
				}
				else
				{
					res.Success = false;
					res.Message = "Cannot find user to delete";
				}
			}
			catch (Exception ex)
			{
				res.Success = false;
				res.Message = "Error while deleting user: " + ex.Message;
			}

			return res;
		}

		public async Task<Response<User>> GetUserAsync(int ID)
		{
			if (ID < 0)
			{
				return new Response<User>()
				{
					Data = null,
					Message = "ID cannot be < 0",
					Success = false
				};
			}

			try
			{
				var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(b => b.Id == ID);
				if (sameIDUser == null)
				{
					return new Response<User>()
					{
						Data = null,
						Message = $"Couldn't find user with ID: {ID}",
						Success = false
					};
				}
				else
				{
					return new Response<User>()
					{
						Data = sameIDUser,
						Message = "Ok",
						Success = true
					};
				}
			}
			catch (Exception)
			{
				return new Response<User>()
				{
					Data = null,
					Message = "Problem with dataseeder library",
					Success = false
				};
			}
		}

		public async Task<Response<User>> AddUserAsync(User user)
		{
			var res = new Response<User>()
			{
				Data = null,
				Message = "",
				Success = false
			};

			var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

			if (sameIDUser != null)
				res.Message = "Found User with same ID, ID must be unique!";
			else if (user == null)
				res.Message = "User couldn't be added - user is null";
			else if (user.Id < 0)
				res.Message = "User couldn't be added - ID cannot be < 0";
			else if (user.FirstName == null || user.FirstName == "")
				res.Message = "User couldn't be added - First Name is empty";
			else if (user.LastName == null || user.LastName == "")
				res.Message = "User couldn't be added - Last Name is empty";
			else if (user.Email == null || user.Email == "")
				res.Message = "User couldn't be added - Email is empty";

			if (res.Message != "")
				return res;

			try
			{
				_dataContext.Users.Add(user);
				await _dataContext.SaveChangesAsync();

				res.Data = user;
				res.Success = true;
				res.Message = "User added successfully";
			}
			catch (Exception ex)
			{
				res.Message = "Error while adding user: " + ex.Message;
			}

			return res;
		}

		public async Task<Response<User>> UpdateUserAsync(User user)
		{
			var res = new Response<User>()
			{
				Data = null,
				Message = "",
				Success = false
			};

			if (user == null)
				res.Message = "User couldn't be updated - user is null";
			else if (user.Id < 0)
				res.Message = "User couldn't be updated - ID cannot be < 0";
			else if (user.FirstName == null || user.FirstName == "")
				res.Message = "User couldn't be updated - First Name is empty";
			else if (user.LastName == null || user.LastName == "")
				res.Message = "User couldn't be updated - Last Name is empty";
			else if (user.Email == null || user.Email == "")
				res.Message = "User couldn't be updated - Email is empty";

			if (res.Message != "")
				return res;

			try
			{
				var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
				if (sameIDUser != null)
				{
					sameIDUser.FirstName = user.FirstName;
					sameIDUser.LastName = user.LastName;
					sameIDUser.Email = user.Email;
					sameIDUser.BirthDate = user.BirthDate;
					await _dataContext.SaveChangesAsync();

					res.Success = true;
					res.Message = "User updated successfully";
				}
				else
				{
					res.Success = false;
					res.Message = "User not found for updating";
				}
			}
			catch (Exception ex)
			{
				res.Success = false;
				res.Message = "Error while updating user: " + ex.Message;
			}
			return res;
		}
    }
}
