using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Model
{
	public class User
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public DateOnly BirthDate { get; set; }
        public List<Transaction> SentTransactions { get; set; }
        public List<Transaction> RecievedTransactions { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
    }
}