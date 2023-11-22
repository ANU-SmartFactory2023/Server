using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class SensorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
