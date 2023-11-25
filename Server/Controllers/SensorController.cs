using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class SensorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // POST pi/<ValuesController>
        [HttpPost( "{id}" )]
		public string Post( int id )
		{
			return id.ToString();
		}

        [HttpPost("{id}")]
        public string Post(int id, SensorModel sensorModel)
        {
            string name = sensorModel.Name;
            string state = sensorModel.State;

            ResponseModel r = new ResponseModel();
            r.msg = "OK";
            r.statusCode = 200;

            return JsonSerializer.Serialize(r);
        }    
    }
}
