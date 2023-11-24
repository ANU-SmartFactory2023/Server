using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    public class resultController1 : ControllerBase
    {
        [HttpGet]
        public string getTesult()
        {
            ResponseModel s = new ResponseModel();

            int grade = 0;
            if(grade == 0)
            s.msg = "OK";
            s.statusCode = 200;

            return JsonSerializer.Serialize(s);
        }
    }
}
