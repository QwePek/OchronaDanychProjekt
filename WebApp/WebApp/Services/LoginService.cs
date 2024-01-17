using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Shared.Login;
using WebApp.Shared.Model;
using WebApp.Shared;
using WebApp.Shared.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace WebApp.Services
{
    public class LoginService : ILoginService
    {
        private readonly DataContext _dataContext;
		PasswordHasher<User> hasher = new PasswordHasher<User>();

		public LoginService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<Response<bool>> RegisterAsync(RegisterModel model)
        {
            var res = new Response<bool>()
            {
                Data = false,
                Message = "",
                Success = false
            };

            if (model == null)
                res.Message = "Model error";
            else if (string.IsNullOrEmpty(model.Email))
                res.Message = "Couldn't register - Email is empty";
            else if (string.IsNullOrEmpty(model.FirstName))
                res.Message = "Couldn't register - First name is empty";
            else if (string.IsNullOrEmpty(model.LastName))
                res.Message = "Couldn't register - Last name is empty";
            else if (model.BirthDate == null)
                res.Message = "Couldn't register - Birth date is empty";
            else if (string.IsNullOrEmpty(model.Password))
                res.Message = "Couldn't register - Password is empty";

            if (res.Message != "")
                return res;

            //Modificating data
            model.Email = char.ToUpper(model.Email.First()) + model.Email.Substring(1).ToLower();
            model.FirstName = char.ToUpper(model.FirstName.First()) + model.FirstName.Substring(1).ToLower();
            model.LastName = char.ToUpper(model.LastName.First()) + model.LastName.Substring(1).ToLower();

            var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (sameIDUser != null)
            {
                res.Success = false;
                res.Message = "User already exist with that email!";
            }
            else
            {
                User newUser = new User();
                newUser.Email = model.Email;
                newUser.FirstName = model.FirstName;
                newUser.LastName = model.LastName;
                newUser.BirthDate = model.BirthDate;
                newUser.PasswordSalt = generateSalt(12);
                newUser.PasswordHash = hashPassword(newUser, model.Password, newUser.PasswordSalt);

				_dataContext.Users.Add(newUser);
                await _dataContext.SaveChangesAsync();

                res.Success = true;
                res.Message = "User registered successfully";
            }
            return res;
        }
        
        public async Task<Response<ClaimsPrincipal>> LoginAsync(LoginModel model)
        {
            var res = new Response<ClaimsPrincipal>()
            {
                Data = null,
                Message = "",
                Success = false
            };

            //Modificating data
            model.Email = model.Email.ToLower();

            User sameEmailUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email);
            if (sameEmailUser != null)
            {
                //Password checking
                if (verifyPassword(sameEmailUser, model.Password, sameEmailUser.PasswordSalt, sameEmailUser.PasswordHash) == PasswordVerificationResult.Success)
                {
					var claims = new List<Claim>
				    {
					    new Claim(ClaimTypes.Email, sameEmailUser.Email),
					    new Claim(ClaimTypes.Name, sameEmailUser.FirstName),
					    new Claim(ClaimTypes.NameIdentifier, sameEmailUser.Id.ToString()),
					    new Claim(ClaimTypes.Surname, sameEmailUser.LastName),
					    new Claim(ClaimTypes.DateOfBirth,sameEmailUser.BirthDate.ToShortDateString())
				    };

					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);

					res.Data = principal;
					res.Success = true;
					res.Message = "Logged in successfully!";
                }
                else
                {
					res.Success = false;
					res.Message = "Wrong password!";
					return res;
				}
            }
            else
            {
                res.Success = false;
                res.Message = "Cannot find user with that email";
            }

            return res;
        }

        private string hashPassword(User user, string password, string salt)
        {
            password += salt;
            return hasher.HashPassword(user, password);
        }

		private PasswordVerificationResult verifyPassword(User user, string providedPassword, string salt, string hashedPassword)
		{
			providedPassword += salt;
			return hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
		}

		public static string generateSalt(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
		}
	}
}
