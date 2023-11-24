using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class processController1 : ControllerBase
    {
        // POST pi/<ValuesController>/2
        [HttpPost("{id}")]
        public string Post(processModel processModel)
        {
            string name = processModel.processName;
            double value = processModel.processValue;

            //중간과정
            //걸린 시간 측정(현재시간 - 시작시간)
            ////? 시작신호 끝신호 받는거를 따로 만들어야하나
            
            //DB에 저장


            ResponseModel s = new ResponseModel();

            //불량 여부 미달 판단
            if (value > 500)
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
    }
}
