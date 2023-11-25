using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        // POST pi/<ValuesController>/2/값
        [HttpPost("{id}")]
        public string Post(int id, SensorModel sensorModel)
        {
            //받아온 값
            string name = sensorModel.Name;
            string state = sensorModel.State;

            //답장용
            ResponseModel r = new ResponseModel();

            if (state == "on")
            {
                //main화면 물체 감지 상태로 변경 (id이용)
                //함수를 쓰면 어떨까? (detectOn(int id))
                if (id == 0) //이거 함수안에 넣어도 될듯?
                {
                    //lot id , 씨리얼 부여 
                    //lot id, 씨리얼 를 이용하여 DB에 데이터 생성
                    //main화면 start버튼 활성화
                }

                //start 버튼 비활성화를 언제하지??
                //물건 보내고 다음 물건 들어오면 눌러야하는데?
                //그전에 누르면 안되는데?
                //센서 1에서 값이 오면 할까?

            }
            else if(state == "off")
            {
                //main화면 물체 없음 상태로 변경
                //이것도 함수 쓸까..?
            }
            else
            {
                //error
            }

            //답장
            r.msg = "OK";
            r.statusCode = 200;

            return JsonSerializer.Serialize(r);
        }    
    }
}
