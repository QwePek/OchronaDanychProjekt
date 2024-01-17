using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Model;

namespace WebApp.Shared.DTO
{
	public class TransactionDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public float Value { get; set; }
		public DateTime Date { get; set; }
		public int SenderId { get; set; }
		public int RecieverId { get; set; }
	}
}
