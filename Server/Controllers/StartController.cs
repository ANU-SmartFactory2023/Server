using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class StartController : Controller
    {
        int i;  
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public string GetResponse()
        {
            ResponseModel r = new ResponseModel();
          
            if(i > 500) // lot 번호 부여 여부 판별
            {
                r.msg = "WAIT";
                r.statusCode = 500;
            }
            else
            {
                r.msg = "OK";
                r.statusCode = 200;
            }
            
            return JsonSerializer.Serialize(r);
        }
    }
}
