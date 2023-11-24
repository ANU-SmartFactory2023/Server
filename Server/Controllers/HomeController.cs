using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Diagnostics;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly SemiconductorContext ProcessDB;

        public HomeController(SemiconductorContext ProcessDB)//(ILogger<HomeController> logger, SemiconductorContext ProcessDB)
        {
            //_logger = logger;
            this.ProcessDB = ProcessDB;
        }

        public IActionResult Index()
        {
            var stdData = ProcessDB.SemiconductorModel.ToList();
            return View(stdData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}