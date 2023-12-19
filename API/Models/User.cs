namespace OchronaDanychProjektAPI.Models
{
	public class User
	{
		string name { get; set; }

		string surname { get; set; }

		string email { get; set; }

		//Passwrod tmp
		//TODO: dodaj do bazy danych xpp
		string password { get; set; }
	}
}
