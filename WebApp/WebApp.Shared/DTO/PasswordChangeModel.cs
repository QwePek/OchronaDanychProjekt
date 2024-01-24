using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared.DTO
{
	public class PasswordChangeModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Old password is required")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "New password is required")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Second factor is required")]
		public string NewSecondFactor { get; set; }
	}
}
