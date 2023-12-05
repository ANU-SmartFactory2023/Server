using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        //DB와 연결, 허브와 연결
        private readonly Total_historyContext ProcessDB;
        private readonly IHubContext<SensorHub> _hubContext;

        public SensorController(Total_historyContext processDB, IHubContext<SensorHub> hubContext)
        {
            ProcessDB = processDB;
            _hubContext = hubContext;
        }

        // POST pi/<ValuesController>/2/값
        [HttpPost("{id}")]
        public async Task<string> Post(int id, SensorModel sensorModel)
        {
            //받아온 값
            string name = sensorModel.sensorName;
            string state = sensorModel.sensorState;

            //답장용
            ResponseModel r = new ResponseModel();
            //답장
            r.msg = "ok";
            r.statusCode = 200;

            if (state == "on")
            {
				////main화면 물체 감지 상태로 변경 (id이용) //함수를 쓰면 어떨까? (detectOn(int id))
				await _hubContext.Clients.All.SendAsync("DetectState",id, "detected");

				if (id == 1)
                {
                    await LotidCreate(); //lot id , 씨리얼 부여  //lot id, 씨리얼 를 이용하여 DB에 데이터 생성

					////main화면 lot id, 씨리얼 번호 띄우기
					await _hubContext.Clients.All.SendAsync("SetLotID", "start");


					////main화면 start버튼 활성화
					await _hubContext.Clients.All.SendAsync("ActivateButton", "startButton");

                    ////start 버튼 비활성화를 언제하지?? -> start 버튼 누르면 or 공정 종료하면
                 
                }

            }
            else if(state == "off")
            {
				////main화면 물체 없음 상태로 변경
				await _hubContext.Clients.All.SendAsync("DetectState", id, "noting");

				if (id == 4) //마지막 공정일 경우 // 수정 필요
				{
					await updateEndtime(); // 전체공정 end time 저장

					////화면에 lotid, 씨리얼 초기화 
					await _hubContext.Clients.All.SendAsync("SetLotID", "end");
				}
			}
            else
            {
				//error
				r.msg = "sentence errer";
				r.statusCode = 404;
			}


            return JsonSerializer.Serialize(r);
        }
		
		/*****************************************************DB Update**************************************************************/

		public async Task LotidCreate() //Lot Id 부여 (데이터 생성)
        {
            DateTime date = DateTime.Now.Date; //오늘 날짜
            string datenum = date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00");

            // 검색
            var semiconductor = ProcessDB.Total_historyModel.Where(c => c.lot_id.Contains(datenum.ToString())).ToList();

			//설정할 lotid
			string lotid = "SC" + datenum;
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

			////Lot Id를 이용햐여 데이터 불러오기 (마지막에 생성된 DB값)
			string lotid = "SC20231205"; //임시
			int serial = 2; //임시

			var updateData = ProcessDB.Total_historyModel.Where(
                x => x.lot_id == lotid && x.serial == serial)
                .FirstOrDefault(); 

            if (updateData == null)
            {
                //예외처리?
                return;
            }
            else
            {
                updateData.end_time = now; //값 변경 
                DateTime daltl = new DateTime(2023, 12, 02, 13, 48, 12); ////아직 시작시간 저장하는게 없어서 임시
                //DateTime starttime = updateData.start_time;
				long timeSpan = (now - daltl).Ticks;
				int min = (int)(timeSpan / (10000000*60));
                double etc = timeSpan % (10000000 * 60);
				int sec = (int)(etc / 10000000);
				double misec = etc % 10000000;
				
				string spendtime = min + ":" + sec + "." + misec;
				updateData.spend_time = spendtime;
				updateData.spend_time = spendtime;

				ProcessDB.Update(updateData); //업데이트
                await ProcessDB.SaveChangesAsync();

            }
        }
        
    }
}
