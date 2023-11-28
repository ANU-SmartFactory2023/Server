using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        //DB와 연결
        private readonly Total_historyContext ProcessDB;
        public SensorController(Total_historyContext processDB)
        {
            ProcessDB = processDB;
        }

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

                if (id == 0)
                {
                    LotidCreate(); //lot id , 씨리얼 부여  //lot id, 씨리얼 를 이용하여 DB에 데이터 생성

                    //main화면 start버튼 활성화
                }

                //start 버튼 비활성화를 언제하지?? -> start 버튼 누르면 하것지

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

        //[HttpGet("{id}")] //이건 빼도  될터인데 테스트용
        public async Task<IActionResult> LotidCreate() //Lot Id 부여 (데이터 생성)
        {
            DateTime date = DateTime.Now.Date; //오늘 날짜
            string datenum = date.Year.ToString()+date.Month.ToString()+date.Day.ToString();

            var Semiconductor = await ProcessDB.Total_historyModel.ToListAsync();  // 기본 목록

            // 검색어로 필터링
            Semiconductor = Semiconductor.Where(c => c.lot_id.Contains(datenum.ToString())).ToList();

            //설정할 lotid
            string lotid = "Semiconductor" + datenum + "01";
            int serial = 0;

            //씨리얼
            //이 날짜에 생산된 것이 있는 지
            if (Semiconductor.Count > 0) //있으면 다음 번호
            {
                serial = Semiconductor.Count + 1;
            }
            else if(Semiconductor.Count == 0) //없으면 새로 만들기
            {
                serial = 1;
            }

            Total_historyModel model = new Total_historyModel();
            model.lot_id = lotid;
            model.serial = serial;

            if (ModelState.IsValid)  //비동기는 쉽지만 남발 할 시 피를 볼 수 있다.
            {
                await ProcessDB.Total_historyModel.AddAsync(model);  
                await ProcessDB.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
