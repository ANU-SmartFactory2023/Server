using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Server
{
    public class SensorHub : Hub
    {
        public async Task SendButtonActivation(string buttonId)
        {
            await Clients.All.SendAsync("ActivateButton", buttonId);
        }
    }
}
