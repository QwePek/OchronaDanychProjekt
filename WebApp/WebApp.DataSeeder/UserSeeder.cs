using Bogus;
using Microsoft.AspNetCore.Identity;
using WebApp.Shared.Model;
using WebApp.Shared.Services;

namespace WebApp.DataSeeder
{
	public class UserSeeder
	{
		public static List<User> GenerateData(int count)
		{
			PasswordHasher<User> hasher = new PasswordHasher<User>();
			int usrID = 1;
			var userFaker = new Faker<User>().UseSeed(123456)
				.RuleFor(x => x.Id, x => usrID++)
				.RuleFor(x => x.FirstName, (u, x) => u.Name.FirstName())
				.RuleFor(x => x.LastName, (u, x) => u.Name.LastName())
				.RuleFor(u => u.PasswordSalt, f => f.Random.AlphaNumeric(12))
				.RuleFor(u => u.PasswordHash, (u, x) => hasher.HashPassword(x, u.Internet.Password() + x.PasswordSalt))
				.RuleFor(x => x.BirthDate, (u, x) => u.Date.PastDateOnly(20, new DateOnly(2023, 10, 10)))
				.RuleFor(x => x.Email, (u, x) => u.Internet.Email(x.FirstName, x.LastName).ToLower());

			return userFaker.Generate(count).ToList();
		}
	}
}
