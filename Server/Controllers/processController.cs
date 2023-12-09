using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
		//DB와 연결, 허브와 연결
		private readonly Total_historyContext ProcessDB;
		private readonly IHubContext<SensorHub> _hubContext;

		public processController(Total_historyContext processDB, IHubContext<SensorHub> hubContext)
		{
			ProcessDB = processDB;
			_hubContext = hubContext;
		}

		// 센서값 저장 및 불량여부 기준 센서값 판단
		// POST pi/<ValuesController>/2/값
		[HttpPost("{id}")]
        public async Task<string> setData(int id, processModel processModel)
        {
            //받은 값
            string cmd = processModel.processCmd;
            string name = processModel.processName;
            double value = processModel.processValue;

            //답장용
            ResponseModel s = new ResponseModel();

            if (cmd == "start")
            {
                await updateDB(cmd, id); //process 데이터 생성, 시작 시간 DB에 저장
                await updateDB("setid", id); //idx 저장
				////main화면 공정 작동중으로 변경
				await _hubContext.Clients.All.SendAsync("WorkingState", name, "working");

				s.msg = "ok";
                s.statusCode = 200;
            }
            else if(cmd == "end")
            {
                //종료시간 DB에 저장, 소요시간 계산, 소요시간 DB에 저장, 센서값 DB에 저장
                await updateDB(cmd, id, null, value);

				////센서값 화면에 표시
				await _hubContext.Clients.All.SendAsync("setValue", name, "setValue");

				bool defective = false;
				////불량 여부 판단


				if (defective == true) //불량품
                {
                    //등급외 DB 저장
                    await updateDB("grade", id, "등외");

					////화면에 lotid, 씨리얼 초기화 
					await _hubContext.Clients.All.SendAsync("SetLotID", "end");

					////전체이력, 개별이력, 등급, (오늘 총 생산량, 전체 불량률) 화면 업데이트
					await _hubContext.Clients.All.SendAsync("SetList", "reload");


					s.msg = "fail";
                    s.statusCode = 200;
                }
                else if(defective == false){ //양품
                    if(id == 4) // 마지막 공정일 경우  -> 수정해야함
					{
                        string grade = "A";
						////등급 판단 //등급 A, B, C, D


						//등급값 DB저장
						await updateDB("grade", id, grade);

                        ////왼쪽 오른쪽 판단 //AB 왼쪽 DC 오른쪽?
                        //결과 
                        if (grade == "A" || grade == "B")
                        {
                            s.msg = "left";
                            s.statusCode = 200;
                        }
                        else if (grade == "C" || grade == "D")
                        {
                            s.msg = "right";
                            s.statusCode = 200;
                        }
                        else
                        {
							//error
							s.msg = "sentence errer";
							s.statusCode = 404;
						}

						////화면에 lotid, 씨리얼 초기화 
						await _hubContext.Clients.All.SendAsync("SetLotID","end");

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
					s.msg = "sentence errer";
					s.statusCode = 404;
				}
				////(불량여부,소요시간 등 화면에 표시) (할지말지 안정함)
				////main화면 공정 끝으로 변경
				await _hubContext.Clients.All.SendAsync("WorkingState", name, "end");
			}
            else
            {
                //error
                s.msg = "sentence error";
                s.statusCode = 404;
            }

            return JsonSerializer.Serialize(s);
        }

		[HttpGet("{id}")]
		public string GetMessage()
		{
			ResponseModel r = new ResponseModel();

			//Lot Id를 이용햐여 데이터 불러오기 (마지막에 생성된 DB값)
			string lotid = ""; //임시
			int serial = 0; //임시
			if (ProcessDB.Total_historyModel.Any()) // 컬렉션이 비어있지 않은지 확인
			{
				var lastData = ProcessDB.Total_historyModel.OrderBy(item => item.idx).Last();
				lotid = lastData.lot_id;
				serial = lastData.serial;
			}

			bool defective = false; ////공정 2가 양품이냐 불량이냐 (임시)
			//DB에서 공정2의 값 가져오기
            var getProcess2Velue = ProcessDB.Process2Model.FirstOrDefault(x => x.lot_id == lotid && x.serial == serial);
			if (getProcess2Velue == null)
			{
                //error
				//return;
			}

			////양품, 불량 판단
            // if (getProcess2Velue.value < 불량판단값)....

			if (defective == false) //양품
			{
				r.msg = "pass";
				r.statusCode = 200;
			}
			else                    //불량품
			{
				r.msg = "fail";
				r.statusCode = 200;
			}

			return JsonSerializer.Serialize(r);
		}

		/*****************************************************DB Update**************************************************************/
		public async Task updateDB(string mode, int id, string? grade = null, double? value = null) //공정 데이터 생성
        {
			//Lot Id를 이용햐여 데이터 불러오기 (마지막에 생성된 DB값)
			string lotid = ""; //임시
			int serial = 0; //임시
			// 컬렉션이 비어있지 않은지 확인
			if (ProcessDB.Total_historyModel.Any())
			{
				var lastData = ProcessDB.Total_historyModel.OrderBy(item => item.idx).Last();
				lotid = lastData.lot_id;
				serial = lastData.serial;
			}
			else
			{
				return;
			}

			switch (mode)
            {
                case "start":
                    switch (id)
                    {
                        case 1:
                            await ProcessModelCreate<Process1Model>(lotid, serial);
                            break;
                        case 2:
                            await ProcessModelCreate<Process2Model>(lotid, serial);
                            break;
                        case 3:
                            await ProcessModelCreate<Process3Model>(lotid, serial);
                            break;
                        default:
                            await ProcessModelCreate<Process4Model>(lotid, serial);
                            break;
                    }
                    break;
                case "setid":
                    switch (id)
                    {
                        case 1:
                            await updateProcessidx<Process1Model>(lotid, serial, id);
                            break;
                        case 2:
                            await updateProcessidx<Process2Model>(lotid, serial, id);
                            break;
                        case 3:
                            await updateProcessidx<Process3Model>(lotid, serial, id);
                            break;
                        default:
                            await updateProcessidx<Process4Model>(lotid, serial, id);
                            break;
                    }
                    break;
                case "end":
                    switch (id)
                    {
                        case 1:
                            await updateEnd<Process1Model>(mode, lotid, serial,value);
                            break;
                        case 2:
                            await updateEnd<Process2Model>(mode, lotid, serial, value);
                            break;
                        case 3:
                            await updateEnd<Process3Model>(mode, lotid, serial, value);
                            break;
                        default:
                            await updateEnd<Process4Model>(mode, lotid, serial, value);
                            break;
                    }
                    break;
                case "grade":
                    await updateGrade<Total_historyModel>(lotid, serial, grade);
                    break;
                default:
                    break;
            }
            


        }
        private async Task ProcessModelCreate<T>(string lotid, int serial) where T : ProcessBaseModel, new()
        {
            T model = new T 
            {
                lot_id = lotid,
                serial = serial,
                start_time = DateTime.Now //시작 시간 저장
            };
           
            ProcessDB.Set<T>().Add(model);
            await ProcessDB.SaveChangesAsync();

        }
        
        private async Task updateProcessidx<T>(string lotid, int serial, int id) where T : ProcessBaseModel, new()
        {
            //process idx 저장
            var getprocessidx = ProcessDB.Set<T>().FirstOrDefault(x => x.lot_id == lotid && x.serial == serial);
            var updateData = ProcessDB.Total_historyModel.FirstOrDefault(c => c.lot_id == lotid && c.serial == serial);

            if (updateData == null || getprocessidx == null)
            {
                return;
            }

            // id 값에 따라 올바른 processX_idx를 업데이트합니다.
            switch (id)
            {
                case 1:
                    updateData.process1_idx = getprocessidx.idx;
                    break;
                case 2:
                    updateData.process2_idx = getprocessidx.idx;
                    break;
                case 3:
                    updateData.process3_idx = getprocessidx.idx;
                    break;
                case 4:
                    updateData.process4_idx = getprocessidx.idx;
                    break;
                default:
                    // 예상치 못한 id 값에 대한 로그를 남기거나 예외를 throw합니다.
                    break;
            }

            ProcessDB.Update(updateData);
            await ProcessDB.SaveChangesAsync();

        }
        
        public async Task updateEnd<T>(string mode, string lotid, int serial, double? value = null) where T : ProcessBaseModel, new()
        {
            var updateData = ProcessDB.Set<T>().FirstOrDefault(x => x.lot_id == lotid && x.serial == serial);

            if (updateData == null)
            {
                //예외처리
                return;
            }

            DateTime now = DateTime.Now;
            updateData.end_time = now;

            long timeSpan = (now - updateData.start_time).Ticks;
			int sec = (int)timeSpan/10000000;
            double misec = timeSpan%10000000;
            string spendtime = sec + "." + misec;
            updateData.spend_time = spendtime;
            updateData.value = value;
              
            ProcessDB.Update(updateData);
            await ProcessDB.SaveChangesAsync();
        }

        public async Task updateGrade<T>(string lotid, int serial, string grade) where T : Total_historyModel, new()
        {
            var updateData = ProcessDB.Set<T>().FirstOrDefault(x => x.lot_id == lotid && x.serial == serial);

            if (updateData == null)
            {
                //예외처리
                return;
            }

            updateData.grade = grade;

            
            ProcessDB.Update(updateData);
            await ProcessDB.SaveChangesAsync();
            

        }

    }
}
