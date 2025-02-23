using Microsoft.AspNetCore.SignalR;

namespace MoveitApi.SignalR
{
    public class FileObserverHub : Hub
    {
        public async Task NotifyFileUploaded(string filepath)
        {
            await Clients.All.SendAsync("FileUploaded", filepath);
        }

        public async Task UpdateUI(string message)
        {
            await Clients.All.SendAsync("UpdateUI", message);
        }
    }
}
