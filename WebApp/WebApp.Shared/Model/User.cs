using System.Text.Json.Serialization;

public enum Role
{
	Admin, User
}

namespace WebApp.Shared.Model
{
	public class User
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string CardNumber { get; set; }
		public string DocumentNumber { get; set; }
		public DateOnly BirthDate { get; set; }
        public List<Transaction> SentTransactions { get; set; }
        public List<Transaction> RecievedTransactions { get; set; }
		public Role Role { get; set; }
		[JsonIgnore]
		public string PasswordHash { get; set; }
		[JsonIgnore]
		public string PasswordSalt { get; set; }
		[JsonIgnore]
		public string SecondFactorEncrypted { get; set; }
		[JsonIgnore]
		public uint LoginAttempts { get; set; } = 0;
		[JsonIgnore]
		public DateTime LoginBan { get; set; } = DateTime.MinValue;
	}
}