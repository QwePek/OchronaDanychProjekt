using Microsoft.AspNetCore.Mvc;

namespace OchronaDanychProjektAPI.Controllers
{
	public class LoginController : Controller
	{
		[Route("/login")]
		public IActionResult Index()
		{
			return View("Login");
		}
	}
}
