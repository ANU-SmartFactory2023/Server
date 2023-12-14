using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    public class SensorHub : Hub
    {
        public async Task SendButtonActivation(string buttonState) //버튼
		{
            await Clients.All.SendAsync("ActivateButton", buttonState);
        }
		public async Task SendDetectState(string name, string state) // 센서감지
		{
			await Clients.All.SendAsync("DetectState", name, state);
		}
		public async Task SendWorking(string name, string state) //공정
		{
			await Clients.All.SendAsync("WorkingState", name, state);
		}
		public async Task SendSenSor(string name, string setValue) //센서값
		{
			await Clients.All.SendAsync("setValue", name, setValue);
		}
		public async Task SetLotID(string lotid, string serial) //lot id, Serial
		{
			await Clients.All.SendAsync("SetLotID", lotid, serial);
		}

		public async Task SetList(string state) //전체이력, 개별이력, 등급, (오늘 총 생산량, 전체 불량률)
		{
			await Clients.All.SendAsync("SetList", state);
		}
	}
}
