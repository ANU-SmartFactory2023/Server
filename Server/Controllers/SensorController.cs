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
        public async Task<string> Post(int id, SensorModel sensorModel)
        {
            //받아온 값
            string name = sensorModel.Name;
            string state = sensorModel.State;

            //답장용
            ResponseModel r = new ResponseModel();
            //답장
            r.msg = "ok";
            r.statusCode = 200;

            if (state == "on")
            {
                ////main화면 물체 감지 상태로 변경 (id이용) //함수를 쓰면 어떨까? (detectOn(int id))

                if (id == 0)
                {
                    await LotidCreate(); //lot id , 씨리얼 부여  //lot id, 씨리얼 를 이용하여 DB에 데이터 생성

                    ////main화면 start버튼 활성화
                    ////start 버튼 비활성화를 언제하지?? -> start 버튼 누르면 하것지
                }

            }
            else if(state == "off")
            {
                ////main화면 물체 없음 상태로 변경

                if (id == 4)
                {
                    await updateEndtime(); // 전체공정 end time 저장
                }
            }
            else
            {
                //error
                r.msg = "error";
                r.statusCode = 400;
            }


            return JsonSerializer.Serialize(r);
        }

        /*****************************************************DB Update**************************************************************/

        public async Task LotidCreate() //Lot Id 부여 (데이터 생성)
        {
            DateTime date = DateTime.Now.Date; //오늘 날짜
            string datenum = date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00");

            // 검색어로 필터링
            var semiconductor = ProcessDB.Total_historyModel.Where(c => c.lot_id.Contains(datenum.ToString())).ToList();

            //설정할 lotid
            string lotid = "Semiconductor" + datenum + "01"; 
            int serial = 0;

            //씨리얼
            //이 날짜에 생산된 것이 있는 지
            if (semiconductor.Count > 0) //있으면 다음 번호
            {
                serial = semiconductor.Count + 1;
            }
            else if (semiconductor.Count == 0) //없으면 새로 만들기
            {
                serial = 1;
            }

            Total_historyModel model = new Total_historyModel
            {
                lot_id = lotid,
                serial = serial
            };

            ProcessDB.Total_historyModel.Add(model);
            await ProcessDB.SaveChangesAsync();
        }


        //공정 종료 시간 업데이트
        public async Task updateEndtime() //현재시간 업데이트
        {
            DateTime now = DateTime.Now;

            ////Lot Id를 이용햐여 데이터 불러오기 (임시)
            //// id? lot_id? 씨리얼? 뭘로 찾아야하지? 1. DB에서 가져온다,  2. 프로그램에 변수로 저장해 놓는다.
            string lotid = "Semiconductor2023120101";
            int serial = 5;

            var updateData = ProcessDB.Total_historyModel.Where(
                x => x.lot_id == lotid && x.serial == serial)
                .FirstOrDefault(); 

            //화면에 lotid, 씨리얼 초기화 

            if (updateData == null)
            {
                //예외처리?
                return;
            }
            else
            {
                updateData.end_time = now; //값 변경 
                DateTime zero = new DateTime(1400, 01, 01, 00, 00, 00); //임시
                updateData.spend_time = zero + (now - new DateTime(2023, 11, 30, 11, 14, 38));  ////아직 시작시간 저장하는게 없어서 임시
                ////updateData.spend_time = zero + (now - updateData.start_time); ////아직 시작시간 저장하는게 없어서

                ProcessDB.Update(updateData); //업데이트
                await ProcessDB.SaveChangesAsync();

            }
        }
        
    }
}
