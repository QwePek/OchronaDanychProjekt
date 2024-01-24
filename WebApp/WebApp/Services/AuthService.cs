using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Shared.Model;
using WebApp.Shared;
using WebApp.Shared.Services;
using Microsoft.AspNetCore.Identity;
using WebApp.Autorization;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Authentication;
using WebApp.Validators;
using WebApp.Shared.DTO;

namespace WebApp.Services
{
    public class AuthService : IAuthService
    {
		PasswordHasher<User> hasher = new PasswordHasher<User>();
        private readonly DataContext _dataContext;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
		private JWTTokenService _tokenService;
        private IJWTUtils _jwtUtils;

        //Validators
		private readonly IValidator<RegisterModel> _registerModelValidator;
		private readonly IValidator<LoginModel> _loginModelValidator;

        public AuthService(DataContext context, IJWTUtils jwtUtils, HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider,
            JWTTokenService tokenService, IValidator<RegisterModel> registerModelValidator, IValidator<LoginModel> loginModelValidator)
        {
            _dataContext = context;
            _jwtUtils = jwtUtils;
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
			_tokenService = tokenService;
            _registerModelValidator = registerModelValidator;
			_loginModelValidator = loginModelValidator;
        }

		public async Task<Response> RegisterAsync(RegisterModel model)
        {
            var validate = _registerModelValidator.Validate(model);
			if (!validate.Success)
				return validate;

            //Modificating data for easier searching
            model.Email = char.ToUpper(model.Email.First()) + model.Email.Substring(1).ToLower();
            model.FirstName = char.ToUpper(model.FirstName.First()) + model.FirstName.Substring(1).ToLower();
            model.LastName = char.ToUpper(model.LastName.First()) + model.LastName.Substring(1).ToLower();

            var sameIDUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (sameIDUser != null)
            {
				return new Response() {
					Message = "User already exist with that email!",
                    Success = false
				};
            }
            else
            {
                User newUser = new User();
                newUser.Email = model.Email;
                newUser.FirstName = model.FirstName;
                newUser.LastName = model.LastName;
                newUser.BirthDate = model.BirthDate;
                newUser.PasswordSalt = AESUtils.generateSalt(12);
                newUser.PasswordHash = AESUtils.hashPassword(hasher, newUser, model.Password, newUser.PasswordSalt);

				_dataContext.Users.Add(newUser);
                await _dataContext.SaveChangesAsync();

                User registeredUser = _dataContext.Users.SingleOrDefault(u => u.Email.ToLower() == model.Email);
                var jwtToken = _jwtUtils.GenerateJwtToken(registeredUser);
                _tokenService.Token = jwtToken;
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(registeredUser);

                return new Response() {
					Message = "User registered successfully",
					Success = true
				};
			}
        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginModel model)
        {
			var validate = _loginModelValidator.Validate(model);

            //Jezeli jest to SecondFactorError to lecimy dalej
            if (!validate.Success && validate.Message != "SecondFactorError")
				return new Response<LoginResponse> {
					Data = null,
					Success = validate.Success,
					Message = validate.Message
				};

			model.Email = model.Email.ToLower();

            User foundUser = _dataContext.Users.SingleOrDefault(u => u.Email.ToLower() == model.Email);
            if(foundUser.LoginBan > DateTime.Now)
                return new Response<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "LimitLogowan"
                };

            if (foundUser == null || AESUtils.verifyPassword(hasher, foundUser, model.Password, foundUser.PasswordSalt, foundUser.PasswordHash) == PasswordVerificationResult.Failed)
            {
                foundUser.LoginAttempts++;
                //Przekroczono ilosc prob logowan
                if (foundUser.LoginAttempts > 3)
                {
                    foundUser.LoginAttempts = 0;
                    foundUser.LoginBan = DateTime.Now.AddSeconds(30);
                }
                await _dataContext.SaveChangesAsync();

                return new Response<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "Wrong password or login"
                };
            }

            foundUser.LoginAttempts = 0;
            await _dataContext.SaveChangesAsync();

            if (string.IsNullOrEmpty(model.SecondFactorPassword))
                return new Response<LoginResponse>
                {
                    Data = null,
                    Success = validate.Success,
                    Message = validate.Message
                };

            //Validate second Factor
            if(!IsSecondFactorPasswordOK(model.SecondFactorPassword, foundUser))
                return new Response<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "Wrong second factor password"
                };

            var jwtToken = _jwtUtils.GenerateJwtToken(foundUser);
			_tokenService.Token = jwtToken;
			((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(foundUser);

            return new Response<LoginResponse> {
                Data = new LoginResponse(foundUser, jwtToken),
                Success = true,
                Message = "Logged in successfully"
            };
        }

        private bool IsSecondFactorPasswordOK(string providedPassword, User user)
        {
            List<int> checkChars = new List<int>();
            for (int i = 0; i < providedPassword.Length; i++)
                if (providedPassword[i] != '*')
                    checkChars.Add(i);

            string secondFactorEncrypted = user.SecondFactorEncrypted;
            string secondFactorDecrypted = AESUtils.Decrypt(secondFactorEncrypted);
            foreach (int l in checkChars)
                if (providedPassword.ElementAt(l) != secondFactorDecrypted.ElementAt(l))
                    return false;

            return true;
        }

        public async Task Logout()
        {
            _tokenService.Token = null;
			((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
			_httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<Response<LoginResponse>> Authenticate(LoginModel model)
        {
            model.Email = model.Email.ToLower();

            User foundUser = _dataContext.Users.SingleOrDefault(u => u.Email.ToLower() == model.Email);
            if (foundUser.LoginBan > DateTime.Now)
                return new Response<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "LimitLogowan"
                };

            if (foundUser == null || AESUtils.verifyPassword(hasher, foundUser, model.Password, foundUser.PasswordSalt, foundUser.PasswordHash) == PasswordVerificationResult.Failed)
            {
                foundUser.LoginAttempts++;
                //Przekroczono ilosc prob logowan
                if (foundUser.LoginAttempts > 3)
                {
                    foundUser.LoginAttempts = 0;
                    foundUser.LoginBan = DateTime.Now.AddSeconds(30);
                }
                await _dataContext.SaveChangesAsync();

                return new Response<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "Wrong password or login"
                };
            }

            foundUser.LoginAttempts = 0;
            await _dataContext.SaveChangesAsync();

            var jwtToken = _jwtUtils.GenerateJwtToken(foundUser);
			return new Response<LoginResponse> {
				Data = new LoginResponse(foundUser, jwtToken),
				Success = true,
				Message = "Authenticated successfully"
			};
        }
	}
}


//public async Task<Response<ClaimsPrincipal>> LoginAsync(LoginModel model)
//{
//    var res = new Response<ClaimsPrincipal>()
//    {
//        Data = null,
//        Message = "",
//        Success = false
//    };

//    //Modificating data
//    model.Email = model.Email.ToLower();

//    User sameEmailUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email);
//    if (sameEmailUser != null)
//    {
//        //Password checking
//        if (verifyPassword(sameEmailUser, model.Password, sameEmailUser.PasswordSalt, sameEmailUser.PasswordHash) == PasswordVerificationResult.Success)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Email, sameEmailUser.Email),
//                new Claim(ClaimTypes.Name, sameEmailUser.FirstName),
//                new Claim(ClaimTypes.NameIdentifier, sameEmailUser.Id.ToString()),
//                new Claim(ClaimTypes.Surname, sameEmailUser.LastName),
//                new Claim(ClaimTypes.DateOfBirth,sameEmailUser.BirthDate.ToShortDateString())
//            };

//            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            var principal = new ClaimsPrincipal(identity);

//            res.Data = principal;
//            res.Success = true;
//            res.Message = "Logged in successfully!";
//        }
//        else
//        {
//            res.Success = false;
//            res.Message = "Wrong password!";
//            return res;
//        }
//    }
//    else
//    {
//        res.Success = false;
//        res.Message = "Cannot find user with that email";
//    }

//    return res;
//}