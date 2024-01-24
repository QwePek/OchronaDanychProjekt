using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApp.Shared.Model
{
	public class Transaction
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public float Value { get; set; }
		public DateTime Date { get; set; }
		public int SenderId { get; set; }
		public int RecieverId { get; set; }
		[JsonIgnore]
		public User Sender { get; set; }
		[JsonIgnore]
		public User Reciever { get; set; }
	}
}