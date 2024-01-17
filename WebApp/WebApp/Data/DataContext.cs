 using WebApp.DataSeeder;
using Microsoft.EntityFrameworkCore;
using WebApp.Shared.Model;

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

			//List<User> users = new List<User>
			//{
			//	new User{Id=11, FirstName= "John",LastName= "Doe",Email=  "email@email.com", PasswordHash="Haslo1!",BirthDate= new DateOnly(1990, 5, 15) },
			//	new User{Id=12,FirstName="Jane",LastName=  "Smith",Email= "jane.smith@email.com",Password= "secret456",BirthDate= new DateOnly(1985, 9, 22) },
			//	new User{Id=13,FirstName="Bob", LastName= "Johnson", Email="bob.johnson@email.com", Password="secure789", BirthDate=new DateOnly(2000, 3, 8) },
			//	new User{Id=14,FirstName="Alice",LastName=  "Williams",Email= "alice.williams@email.com", Password="pass1234", BirthDate=new DateOnly(1993, 12, 5) },
			//	new User{Id=15,FirstName="Charlie",LastName=  "Brown",Email= "charlie.brown@email.com",Password= "brownie456", BirthDate=new DateOnly(1988, 7, 17) },
			//	new User{Id=16,FirstName="Eva", LastName= "Martinez", Email="eva.martinez@email.com", Password="eva123", BirthDate=new DateOnly(1995, 2, 10) },
			//	new User{Id=17,FirstName="David", LastName= "Lee", Email="david.lee@email.com",Password= "lee456",BirthDate= new DateOnly(1992, 6, 30) },
			//	new User{Id=18,FirstName="Sophie", LastName= "Turner",Email= "sophie.turner@email.com", Password="sophie789", BirthDate=new DateOnly(1989, 11, 25) },
			//	new User{Id=19,FirstName="Michael",LastName=  "Wang",Email= "michael.wang@email.com",Password= "wang123",BirthDate= new DateOnly(1997, 8, 12) }
			//};

            //generatedUsers.AddRange(users);
            List<Transaction> generateTransactions = TransactionSeeder.GenerateData(200, generatedUsers);


            modelBuilder.Entity<User>().HasData(generatedUsers);
			modelBuilder.Entity<Transaction>().HasData(generateTransactions);
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