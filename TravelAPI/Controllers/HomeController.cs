using Microsoft.AspNetCore.Mvc;

namespace TravelAPI.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Content("Welcome to TravelAPI!");
        }
    }
}