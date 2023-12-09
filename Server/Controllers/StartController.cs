using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("pi/[controller]")]
    [ApiController]
    public class StartController : ControllerBase
    {
		//DB와 연결, 허브와 연결
		private readonly Total_historyContext ProcessDB;
		private readonly IHubContext<SensorHub> _hubContext;

		public StartController(Total_historyContext processDB, IHubContext<SensorHub> hubContext)
		{
			ProcessDB = processDB;
			_hubContext = hubContext;
		}

		public static bool isPressed; //start 버튼이 눌렸느냐?
		[HttpGet]
        public async Task<string> GetResponse()
        {
            ResponseModel r = new ResponseModel();

            if(isPressed == true)
            {
				r.msg = "ok";
				r.statusCode = 200;
			}
            else
            {
				////main화면 start버튼 활성화
				await _hubContext.Clients.All.SendAsync("ActivateButton", "startButton"); //테스트용
				r.msg = "wait";
				r.statusCode = 500;
            }

            return JsonSerializer.Serialize(r);
        }

		[HttpPost("toggle")]
		public async Task ToggleIsPressed()
		{
            isPressed = !isPressed;

			if(isPressed == true)
			{
				//시작시간 추가
				DateTime now = DateTime.Now;
				//Lot Id를 이용햐여 데이터 불러오기 (마지막에 생성된 DB값)
				string lotid = ""; //임시
				int serial = 0; //임시
				if (ProcessDB.Total_historyModel.Any()) // 컬렉션이 비어있지 않은지 확인
				{
					var lastData = ProcessDB.Total_historyModel.OrderBy(item => item.idx).Last();
					lotid = lastData.lot_id;
					serial = lastData.serial;
				}
				else
				{
					return;
				}

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
					updateData.start_time = now; //값 변경 

					ProcessDB.Update(updateData); //업데이트
					await ProcessDB.SaveChangesAsync();

				}
			}

		}

	}
}
