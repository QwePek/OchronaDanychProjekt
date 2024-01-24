using WebApp.Shared.Model;

namespace WebApp.Shared.DTO
{
    public class LoginResponse
    {
        public LoginResponse(User user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            BirthDate = user.BirthDate;
            SentTransactions = user.SentTransactions;
            RecievedTransactions = user.RecievedTransactions;
            Role = user.Role;
            Token = token;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }
        public List<Transaction> SentTransactions { get; set; }
        public List<Transaction> RecievedTransactions { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
    }
}
