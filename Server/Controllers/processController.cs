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
        //DB와 연결
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
            //받은 값
            string cmd = processModel.processCmd;
            string name = processModel.processName;
            double value = processModel.processValue;

            //답장
            ResponseModel s = new ResponseModel();

            //중간과정
            //DB에 저장
            saveDBtest(id); //비동기 괜찮나?

            if (cmd == "start")
            {
                //시작 시간 DB에 저장
            }
            else if(cmd == "end")
            {
                //종료시간 DB에 저장
                //소요시간 계산
                //소요시간 DB에 저장
                //센서값 DB에 저장
                //불량 여부 판단
                bool defective = true;
                if (defective == true) //불량품
                {
                    s.msg = "fail";
                    s.statusCode = 200;
                }
                else if(defective == false){ //양품
                    if(id == 4) // 마지막 공정일 경우
                    {
                        //등급 판단 //등급 A, B, C, D
                        string gread = "A";
                        //왼쪽 오른쪽 판단 //AB 왼쪽 DC 오른쪽?
                        //결과 
                        if (gread == "A" || gread == "B")
                        {
                            s.msg = "left";
                            s.statusCode = 200;
                        }
                        else if (gread == "C" || gread == "D")
                        {
                            s.msg = "right";
                            s.statusCode = 200;
                        }
                        else
                        {
                            //error
                        }
                    }
                    else
                    {
                        s.msg = "pass";
                        s.statusCode = 200;
                    }
                }
                else
                {
                    //error
                }

                if (id == 4 )//&& 양품)
                {
                    
                }
                else
                {
                    //결과
                }
                
            }
            else
            {
                //error
            }

            

            //불량 여부 미달 판단
            if (id == 4 && value > 500) // 앞으로의 결정에 따라 없어질 수도 있는 과정 입니다.
            {
                // 등급공정 판단 과정

                

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
        public async Task<IActionResult> saveDBtest(int id)
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
