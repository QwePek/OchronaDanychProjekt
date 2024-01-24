 using WebApp.DataSeeder;
using Microsoft.EntityFrameworkCore;
using WebApp.Shared.Model;
using Microsoft.AspNetCore.Identity;
using WebApp.Shared;

namespace WebApp.Data
{
	public class DataContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Transaction> Transactions { get; set; }

		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            // Fluent API dla klasy User
            modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(u => u.Id);
				entity.Property(u => u.FirstName).IsRequired();
				entity.Property(u => u.LastName).IsRequired();
				entity.Property(u => u.Email).IsRequired();
				entity.Property(u => u.BirthDate).IsRequired();

				entity.HasMany(u => u.SentTransactions)
				.WithOne(t => t.Sender)
				.HasForeignKey(t => t.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasMany(u => u.RecievedTransactions)
					.WithOne(t => t.Reciever)
					.HasForeignKey(t => t.RecieverId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.Property(u => u.PasswordHash).IsRequired();
				entity.Property(u => u.PasswordSalt).IsRequired();

				entity.Property(u => u.SecondFactorEncrypted).IsRequired();
				entity.Property(u => u.LoginAttempts).IsRequired();

				entity.Property(u => u.CardNumber).IsRequired();
				entity.Property(u => u.DocumentNumber).IsRequired();
			});

			// Fluent API dla klasy Message
			modelBuilder.Entity<Transaction>(entity =>
			{
				entity.HasKey(m => m.Id);
				entity.Property(t => t.Title).IsRequired();
				entity.Property(m => m.Content).IsRequired();
				entity.Property(m => m.Date).IsRequired();
				entity.Property(t => t.Value).IsRequired();

				entity.HasOne(t => t.Sender)
					.WithMany(u => u.SentTransactions)
					.HasForeignKey(t => t.SenderId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(t => t.Reciever)
					.WithMany(u => u.RecievedTransactions)
					.HasForeignKey(t => t.RecieverId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			List<User> generatedUsers = UserSeeder.GenerateData(10);

            PasswordHasher<User> hasher = new PasswordHasher<User>();
			string saltForAdmin = generateSalt(1);

			//WARNING
			AESUtils.KEY = "0123456789abcdef";
			AESUtils.IV = "abcdef0123456789";

			List<User> users = new List<User>
			{
				new User{ Id = 11, FirstName= "Admin", LastName= "AdminLast", Email =  "admin@admin.pl",
					PasswordHash = hasher.HashPassword(null, "Qwer1234!" + saltForAdmin),
					PasswordSalt=saltForAdmin, BirthDate= new DateOnly(2020, 3, 12), Role=Role.Admin,
					SecondFactorEncrypted = AESUtils.Encrypt("12345"), CardNumber = AESUtils.Encrypt("123456789876"),
					DocumentNumber = AESUtils.Encrypt("123456789") },

				new User{ Id = 12, FirstName= "Filip", LastName= "Bochra", Email =  "filip@eweb.pl", 
					PasswordHash = hasher.HashPassword(null, "Qwer1234!" + saltForAdmin), 
					PasswordSalt = saltForAdmin, BirthDate = new DateOnly(1990, 5, 15), Role=Role.User, 
					SecondFactorEncrypted = AESUtils.Encrypt("54321"), CardNumber=AESUtils.Encrypt("676331231234"),
                    DocumentNumber = AESUtils.Encrypt("987654321") }
			};

			generatedUsers.AddRange(users);
			List<Transaction> generateTransactions = TransactionSeeder.GenerateData(200, generatedUsers);

            modelBuilder.Entity<User>().HasData(generatedUsers);
			modelBuilder.Entity<Transaction>().HasData(generateTransactions);
		}

		//DEBUG
        public static string generateSalt(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }
    }
}

// instalacja dotnet ef 
//dotnet tool install --global dotnet-ef

// aktualizacja 
//dotnet tool update --global dotnet-ef

// dotnet ef migrations add [nazwa_migracji]
// dotnet ef database update 


// cofniecie migraji niezaplikowanych 
//dotnet ef migrations remove