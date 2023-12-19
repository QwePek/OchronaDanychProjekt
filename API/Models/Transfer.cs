namespace OchronaDanychProjektAPI.Models
{
	public class Transfer
	{
		int ID { get; set; }

		float value { get; set; }

		string title { get; set; }

		User sender { get; set; }
	}
}
