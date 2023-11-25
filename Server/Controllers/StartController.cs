using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class StartController : ControllerBase
    {

        [HttpGet]
        public string GetResponse()
        {
            ResponseModel r = new ResponseModel();
          
            bool isPressed = false; //start 버튼이 눌렸느냐?

            if(isPressed == false)
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
