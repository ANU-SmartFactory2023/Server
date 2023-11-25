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
            // 각각의 데이터를 불러옴
            var th = ProcessDB.Total_historyModel.ToList();
            var p1 = ProcessDB.Process1Model.ToList();
            var p2 = ProcessDB.Process2Model.ToList();
            var p3 = ProcessDB.Process3Model.ToList();
            var p4 = ProcessDB.Process4Model.ToList();

            // 데이터를 뷰 모델에 저장
            var viewModel = new MainModel
            {
                Total_historyData = th,
                process1Data = p1,
                process2Data = p2,
                process3Data = p3,
                process4Data = p4,
            };
            return View(viewModel);
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