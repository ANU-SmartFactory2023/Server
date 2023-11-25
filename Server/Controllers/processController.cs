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
        // POST pi/<ValuesController>/2/값
        [HttpPost("{id}")]
        public string setData(int id, processModel processModel)
        {
            //받은 값
            string cmd = processModel.processCmd;
            string name = processModel.processName;
            double value = processModel.processValue;

            //답장용
            ResponseModel s = new ResponseModel();

            //중간과정
            //DB에 저장
            saveDBtest(id); //비동기 괜찮나?

            //전체 공정 시작, 종료 DB저장 타이밍
            //시작 타이밍 사용자가 Start를 누른 시점
            //종료 타이밍 등급판정 fail 혹은 마지막 공정의 "센서"상태가 off가 된 시점

            if (cmd == "start")
            {
                //시작 시간 DB에 저장
                //main화면 공정 작동중으로 변경
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
                    //등급외 DB 저장
                    s.msg = "fail";
                    s.statusCode = 200;
                }
                else if(defective == false){ //양품
                    if(id == 4) // 마지막 공정일 경우
                    {
                        //등급 판단 //등급 A, B, C, D
                        string gread = "A";
                        //등급값 DB저장

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
                //센서값 화면에 표시 (불량여부,소요시간 등도 가능)
                //main화면 공정 끝으로 변경
            }
            else
            {
                //error
            }

            return JsonSerializer.Serialize(s);
        }

        //DB 저장 test
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
            //업데이트 시에 새로 창을 불러오는 법 밖에 없나?
            //안불러오는 방법이 있다 방법을 찾아라
        }
    }
}
