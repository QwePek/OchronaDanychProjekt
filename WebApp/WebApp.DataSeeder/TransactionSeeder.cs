using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Model;

namespace WebApp.DataSeeder
{
	public class TransactionSeeder
	{
		public static List<Transaction> GenerateData(int count, List<User> users)
		{
			int transaction = 1;
			var transactionFaker = new Faker<Transaction>().UseSeed(123456)
				.RuleFor(x => x.Id, x => transaction++)
				.RuleFor(t => t.Title, f => f.Lorem.Word())
				.RuleFor(x => x.Content, (u, x) => u.Lorem.Sentence())
				.RuleFor(x => x.Date, (u, x) => u.Date.Past(20, new DateTime(2023, 10, 10)))
				.RuleFor(t => t.Value, f => f.Random.Float(0, 1000))
				.RuleFor(x => x.SenderId, (u, x) => u.Random.Int(1, users.Count))
				.RuleFor(x => x.RecieverId, (u, x) =>
				{
					int recieverId;
					do
					{
						recieverId = u.Random.Int(1, users.Count);
					} while (recieverId == x.SenderId);

					return recieverId;
				});

			return transactionFaker.Generate(count).ToList();
		}
	}
}
