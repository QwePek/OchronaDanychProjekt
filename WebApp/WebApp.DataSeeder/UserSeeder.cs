using Bogus;
using Microsoft.AspNetCore.Identity;
using System.Text;
using WebApp.Shared;
using WebApp.Shared.Model;
using WebApp.Shared.Services;

namespace WebApp.DataSeeder
{
	public class UserSeeder
	{
		public static List<User> GenerateData(int count)
		{
            PasswordHasher <User> hasher = new PasswordHasher<User>();
			int usrID = 1;
			var userFaker = new Faker<User>().UseSeed(123456)
				.RuleFor(x => x.Id, x => usrID++)
				.RuleFor(x => x.FirstName, (u, x) => u.Name.FirstName())
				.RuleFor(x => x.LastName, (u, x) => u.Name.LastName())
				.RuleFor(x => x.Role, f => f.PickRandom<Role>())
				.RuleFor(u => u.PasswordSalt, f => f.Random.AlphaNumeric(12))
				.RuleFor(u => u.PasswordHash, (u, x) => hasher.HashPassword(x, u.Internet.Password() + x.PasswordSalt))
				.RuleFor(x => x.BirthDate, (u, x) => u.Date.PastDateOnly(20, new DateOnly(2023, 10, 10)))
				.RuleFor(x => x.Email, (u, x) => u.Internet.Email(x.FirstName, x.LastName).ToLower())
				.RuleFor(x => x.SecondFactorEncrypted, (u, x) => AESUtils.Encrypt(GenerateRandomNumber(5)))
				.RuleFor(x => x.CardNumber, (u, x) => AESUtils.Encrypt(GenerateRandomNumber(12)))
				.RuleFor(x => x.DocumentNumber, (u, x) => AESUtils.Encrypt(GenerateRandomNumber(9)));

            return userFaker.Generate(count).ToList();
		}

        private static string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
                stringBuilder.Append(random.Next(0, 10));

            return stringBuilder.ToString();
        }
    }
}
