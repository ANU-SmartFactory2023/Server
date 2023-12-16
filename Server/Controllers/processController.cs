using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Models;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
            value = Math.Round(value, 2);
			Console.WriteLine($"recv post id : {id}, Process cmd:{cmd}, namd:{name}, value:{value}");

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
				await _hubContext.Clients.All.SendAsync("setValue", name, value);

				bool defective = await chackReference(id, value); ////불량 여부 판단 //DB에서 id로 불량품값 가져오기


				if (defective == false) //불량품
                {
                    //등급외 DB 저장
                    await updateDB("grade", id, "D");

					////버튼 초기화
					await _hubContext.Clients.All.SendAsync("ActivateButton", "endbutton");

					//화면 초기화
					await _hubContext.Clients.All.SendAsync("SetList", "reload");


					s.msg = "fail";
                    s.statusCode = 200;
                }
                else if(defective == true){ //양품
					////main화면 공정 끝으로 변경, 수량, 불량률 화면 수정
					await _hubContext.Clients.All.SendAsync("WorkingState", name, "end");

					if (name == "euvLithography") // 마지막 공정일 경우
					{
                        string grade = await chackFinal(); //등급 판단 //등급 A, B, C

						//등급값 DB저장
						await updateDB("grade", id, grade);

                        ////왼쪽 오른쪽 판단 
                        ////DB에서 변수 grade에 있는 등급에 해당하는 왼오값을 가져오기
                        string direction = await getDirection(grade); ; //left or right

						//결과 
						s.msg = direction;
                        s.statusCode = 200;
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
		public async Task<string> GetMessage()
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

			//DB에서 공정2의 값 가져오기
            var getProcess2Velue = ProcessDB.Process2Model.FirstOrDefault(x => x.lot_id == lotid && x.serial == serial);
			if (getProcess2Velue == null)
			{
				r.msg = "fail";
				r.statusCode = 200;
			}
            else
            {
				bool defective = await chackReference(2, (double)getProcess2Velue.value);

				if (defective == true) //양품
				{
					r.msg = "pass";
					r.statusCode = 200;
				}
				else                    //불량품
				{
					r.msg = "fail";
					r.statusCode = 200;
				}
			}

            Console.WriteLine( "Process Get msg : " + r.msg);
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
		//기준값 비교  //true가 불량품
		public async Task<bool> chackReference(int id, double value)
		{
			var checkvalue = ProcessDB.ReferenceModel.FirstOrDefault();
            if( checkvalue == null)
            {
                return false;
            }

            if (id == 1)
            {

				if (value > checkvalue.bottom1)
                {
                    return false; //불량
                }
                else
                {
                    return true; //양품
                }
			}
            else if (id == 2)
            {
				if (value > checkvalue.bottom2)
				{
					return false; //불량
				}
				else
				{
					return true; //양품
				}
			}
            else if(id == 3)
            {
				if (value > checkvalue.bottom3)
				{
					return false; //불량
				}
				else
				{
					return true; //양품
				}
			}
            else if(id == 4)
            {
                //4공정 등급체크시 전체등급체크를 해야하나? 아니면 함수를 따로 만들까??
				if (value > checkvalue.bottom4)
				{
					return false; //불량
				}
				else
				{
					return true; //양품
				}

			}

            return false;
		}
		public async Task<string> chackFinal()
        {
            try
            {
				var checkvalue = ProcessDB.ReferenceModel.FirstOrDefault();
				if (checkvalue == null)
				{
					return "error";
				}

				var lastData = ProcessDB.Total_historyModel.OrderBy(item => item.idx).Last();

				var P1 = ProcessDB.Process1Model.FirstOrDefault(c => c.idx == lastData.process1_idx);
				var P2 = ProcessDB.Process2Model.FirstOrDefault(c => c.idx == lastData.process2_idx);
				var P3 = ProcessDB.Process3Model.FirstOrDefault(c => c.idx == lastData.process3_idx);
				var P4 = ProcessDB.Process4Model.FirstOrDefault(c => c.idx == lastData.process4_idx);

				var final = ( 
                    ( P1 == null ? 0 : P1.value ) + 
                    ( P2 == null ? 0 : P2.value ) + 
                    ( P3 == null ? 0 : P3.value ) + 
                    ( P4 == null ? 0 : P4.value ) 
                    ) / 4;

				if (final > checkvalue.A_final)
				{
					return "A";
				}
				else if (final < checkvalue.A_final && final > checkvalue.B_final)
				{
					return "B";
				}
				else if (final < checkvalue.B_final && final > checkvalue.C_final)
				{
					return "C";
				}
				else if (final < checkvalue.C_final)
				{
					return "D";
				}
				else
				{
					return "error";
				}
			}
            catch (Exception ex)
            {
				return "error " + ex.Message;
			}
        }
        //방향 읽어오기
		public async Task<string> getDirection(string grade)
		{
			var checkvalue = ProcessDB.ReferenceModel.FirstOrDefault();
            if( checkvalue == null )
            {
                return "fail";
            }

			if (grade == "A")
			{
				return checkvalue.A_direction;
			}
			else if (grade == "B")
			{
				return checkvalue.B_direction;
			}
			else if (grade == "C")
			{
				return checkvalue.C_direction;
			}
			else
			{
				return "fail";
			}

		}
	}
}
