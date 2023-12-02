using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Server
{
    public class SensorHub : Hub
    {
        public async Task SendButtonActivation(string buttonState)
		{
            await Clients.All.SendAsync("ActivateButton", buttonState);
        }
		public async Task SendDetectState(int id, string state)
		{
			await Clients.All.SendAsync("DetectState", id, state);
		}
		public async Task SendWorking(int id, string state)
		{
			await Clients.All.SendAsync("WorkingState", id, state);
		}
		public async Task SendSenSor(int id, string setValue)
		{
			await Clients.All.SendAsync("setValue", id, setValue);
		}
	}
}
