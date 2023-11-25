using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Diagnostics;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly Total_historyContext ProcessDB;

        public HomeController(Total_historyContext ProcessDB)//(ILogger<HomeController> logger, Total_historyContext ProcessDB)
        {
            //_logger = logger;
            this.ProcessDB = ProcessDB;
        }

        public IActionResult Index()
        {
            var thmData = ProcessDB.Total_historyModel.ToList();
            return View(thmData);
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