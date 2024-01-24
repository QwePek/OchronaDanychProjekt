using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp.Shared.Model;

namespace WebApp.Autorization
{
    public interface IJWTUtils
    {
        public string GenerateJwtToken(User user);
    }

    public class JWTUtils : IJWTUtils
    {
        private readonly AppSettings _appSettings;

        public JWTUtils(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Name, user.FirstName),
                        new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                        new Claim(JwtRegisteredClaimNames.Birthdate,user.BirthDate.ToShortDateString())
					}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = "https://localhost:7033",
                Issuer = "https://localhost:7033"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static bool IsJwtTokenValid(string jwtToken)
        {
            // Tworzymy handler JWT
            if (jwtToken.Contains("Bearer"))
                jwtToken = jwtToken.Substring(7);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;
                DateTime expirationDate = token?.ValidTo ?? DateTime.MinValue;
                bool isTokenValid = expirationDate > DateTime.UtcNow;
                return isTokenValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsJwtTokenValid(long expirationTimestamp)
        {
            DateTime expirationUtc = DateTimeOffset.FromUnixTimeSeconds(expirationTimestamp).UtcDateTime;
            return expirationUtc > DateTime.UtcNow;
        }
    }
}
