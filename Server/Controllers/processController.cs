using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class processController : ControllerBase
    {
        // 센서값 저장 및 불량여부 기준 센서값 판단
        // POST pi/<ValuesController>/2
        [HttpPost("{id}")]
        public string setData(int id, processModel processModel)
        {
            string name = processModel.processName;
            double value = processModel.processValue;

            //중간과정
            //DB에 저장
            HomeController i = new HomeController(ILogger < HomeController > logger);
            i.Index();
            // id 번의 name칼럼에 value 저장?

            ResponseModel s = new ResponseModel();

            //불량 여부 미달 판단
            if (id == 4 && value > 500) // 앞으로의 결정에 따라 없어질 수도 있는 과정 입니다.
            {
                // 등급공정 판단 과정

                s.msg = "Grade=n"; //등급 A, B, C, D
                s.statusCode = 200;

            }
            else if (value > 500)
            {
                s.msg = "Pass";
                s.statusCode = 200;
            }
            else
            {
                s.msg = "Fail";
                s.statusCode = 200;
            }

            return JsonSerializer.Serialize(s);
        }

        //앞으로의 결정에 따라 없어질 수도 있는 과정 입니다.
        // 시작 신호
        [HttpPost("{id}/start")]
        public string setStartTime(int id, TimeModel timestart)
        {
            string name = timestart.Name;
            DateTime start = timestart.Time;

            //시작시간 DB? 변수? 에 저장

            ResponseModel s = new ResponseModel();

            s.msg = "OK";
            s.statusCode = 200;

            return JsonSerializer.Serialize(s);
        }
        // 종료 신호
        [HttpPost("{id}/end")]
        public string setEndTime(int id, TimeModel timestart)
        {
            string name = timestart.Name;
            DateTime start = timestart.Time;

            //종료시간 DB? 변수? 에 저장

            ResponseModel s = new ResponseModel();

            s.msg = "OK";
            s.statusCode = 200;

            return JsonSerializer.Serialize(s);
        }
    }
}
