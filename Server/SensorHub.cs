using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Server
{
    public class SensorHub : Hub
    {
        public async Task SendButtonActivation(string buttonState) //버튼
		{
            await Clients.All.SendAsync("ActivateButton", buttonState);
        }
		public async Task SendDetectState(int id, string state) // 센서감지
		{
			await Clients.All.SendAsync("DetectState", id, state);
		}
		public async Task SendWorking(int id, string state) //공정
		{
			await Clients.All.SendAsync("WorkingState", id, state);
		}
		public async Task SendSenSor(int id, string setValue) //센서값
		{
			await Clients.All.SendAsync("setValue", id, setValue);
		}
		public async Task SetLotID(string state) //lot id, Serial
		{
			await Clients.All.SendAsync("SetLotID", state);
		}
	}
}
