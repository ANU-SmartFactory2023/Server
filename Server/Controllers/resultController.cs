using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    public class resultController : ControllerBase
    {
        [HttpGet]
        public string getResult()
        {
            //앞으로의 결정에 따라 없어질 수도 있는 클레스 입니다.
            ResponseModel s = new ResponseModel();

            int grade = 0;
            if(grade == 0)
            s.msg = "OK";
            s.statusCode = 200;

            return JsonSerializer.Serialize(s);
        }
    }
}
