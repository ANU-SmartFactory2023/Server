using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class StartController : ControllerBase
    {
        public static bool isPressed; //start 버튼이 눌렸느냐?
		[HttpGet]
        public string GetResponse()
        {
            ResponseModel r = new ResponseModel();

            if(isPressed == true)
            {
				r.msg = "ok";
				r.statusCode = 200;
			}
            else
            {
				r.msg = "wait";
				r.statusCode = 500;
            }

            return JsonSerializer.Serialize(r);
        }

		[HttpPost("toggle")]
		public string ToggleIsPressed()
		{
            isPressed = true;

			ResponseModel r = new ResponseModel();
			r.msg = "Toggled";
			r.statusCode = 200;

			return JsonSerializer.Serialize(r);
		}
	}
}
