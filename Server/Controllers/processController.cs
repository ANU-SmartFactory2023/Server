using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class processController : ControllerBase
    {
        private readonly Total_historyContext ProcessDB;
        public processController(Total_historyContext processDB)
        {
            ProcessDB = processDB;
        }
        // 센서값 저장 및 불량여부 기준 센서값 판단
        // POST pi/<ValuesController>/2
        [HttpPost("{id}")]
        public string setData(int id, processModel processModel)
        {
            string name = processModel.processName;
            double value = processModel.processValue;

            //중간과정
            //DB에 저장
            saveDB(id); //비동기 괜찮나?
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

        //test
        [HttpGet("{id}")] //이건 빼도  될터인데 테스트용
        public async Task<IActionResult> saveDB(int id)
        {
            // SemiconductorModel model = new SemiconductorModel();      수정필요한 코드
            //  model.lot_id = "test"+ id.ToString();                    수정필요한 코드
            //  model.sensor_1 = 1;                                      수정필요한 코드
            //  model.sensor_2 = 2;                                      수정필요한 코드
            //  model.sensor_3 = 3;                                      수정필요한 코드
            //  model.sensor_4 = 4;                                      수정필요한 코드
            //  model.grade = 0;                                         수정필요한 코드
            if (ModelState.IsValid)  //비동기는 쉽지만 남발 할 시 피를 볼 수 있다.
            {
                //    await ProcessDB.SemiconductorModel.AddAsync(model);   수정필요한 코드
                await ProcessDB.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            //await i.Create(model);    

            return RedirectToAction("Index", "Home");
        }
    }
}
