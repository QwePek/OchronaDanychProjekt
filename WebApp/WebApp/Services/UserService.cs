using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Shared;
using WebApp.Shared.DTO;
using WebApp.Shared.Model;
using WebApp.Shared.Services;
using WebApp.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApp.Services
{
	public class UserService : IUserService
	{
		PasswordHasher<User> hasher = new PasswordHasher<User>();
		private readonly DataContext _dataContext;

		//Validators
		private readonly IValidator<User> _userValidator;
		private readonly IValidator<PasswordChangeModel> _passwordChangeModelValidator;

		public UserService(DataContext context, IValidator<User> userValidator, IValidator<PasswordChangeModel> passwordChangeModelValidator)
		{
			_dataContext = context;
			_userValidator = userValidator;
			_passwordChangeModelValidator = passwordChangeModelValidator;
		}

        public async Task<Response<List<User>>> GetUsersAsync()
		{
			//TODO 
			var users = await _dataContext.Users.ToListAsync();

			return new Response<List<User>>()
			{
				Data = users,
				Message = "Ok",
				Success = true
			};
		}

		public async Task<Response> DeleteUserAsync(int ID)
		{
			if (ID < 0)
				return new Response() {
					Success = false,
					Message = "User couldn't be deleted - ID cannot be < 0"
				};

			var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(b => b.Id == ID);
			if (sameIDUser != null)
			{
				_dataContext.Users.Remove(sameIDUser);
				await _dataContext.SaveChangesAsync();

				return new Response() {
					Success = true,
					Message = "User deleted successfully"
				};
			}
			
			return new Response() {
				Success = false,
				Message = "Cannot find user to delete"
			};
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

		public async Task<Response> AddUserAsync(User user)
		{
			var validate = _userValidator.Validate(user);
			if (!validate.Success)
				return validate;

			var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
			string errorMessage = "";

			if (sameIDUser != null)
				return new Response() {
					Success = false,
					Message = "Found User with same ID, ID must be unique!"
				};

			_dataContext.Users.Add(user);
			await _dataContext.SaveChangesAsync();

			return new Response() {
				Message = "User added successfully",
				Success = true
			};
		}

		public async Task<Response> UpdateUserAsync(User user)
		{
			var validate = _userValidator.Validate(user);
			if (!validate.Success)
				return validate;

			var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
			if (sameIDUser != null)
			{
				sameIDUser.FirstName = user.FirstName;
				sameIDUser.LastName = user.LastName;
				sameIDUser.Email = user.Email;
				sameIDUser.BirthDate = user.BirthDate;

				await _dataContext.SaveChangesAsync();

				return new Response() {
					Success = true,
					Message = "User updated successfully"
				};
			}
			return new Response() {
				Success = false,
				Message = "User not found for updating"
			};
		}

		public async Task<Response> ChangePasswordAsync(PasswordChangeModel model)
		{
			var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
			if (sameIDUser != null)
			{
				if (AESUtils.verifyPassword(hasher, sameIDUser, model.OldPassword, sameIDUser.PasswordSalt, sameIDUser.PasswordHash) == PasswordVerificationResult.Failed)
					return new Response()
					{
						Success = false,
						Message = "Old password not correct"
					};

				var validate = _passwordChangeModelValidator.Validate(model);
				if (!validate.Success)
					return validate;

				sameIDUser.PasswordSalt = AESUtils.generateSalt(12);
				sameIDUser.PasswordHash = AESUtils.hashPassword(hasher, sameIDUser, model.NewPassword, sameIDUser.PasswordSalt);
				sameIDUser.SecondFactorEncrypted = AESUtils.Encrypt(model.NewSecondFactor);

				await _dataContext.SaveChangesAsync();

				return new Response()
				{
					Success = true,
					Message = "Password updated successfully"
				};
			}

			return new Response() {
				Success = false,
				Message = "TRYING TO CHANGE PASSWORD TO NOT AUTHENTICATED USER!!!"
			};
		}
	}
}
