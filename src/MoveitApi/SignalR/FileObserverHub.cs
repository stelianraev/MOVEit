using Microsoft.AspNetCore.SignalR;

namespace MoveitApi.SignalR
{
    public class FileObserverHub : Hub
    {
        public async Task NotifyFileUploaded(string fileName)
        {
            await Clients.All.SendAsync("FileUploaded", fileName);
        }

        public async Task UpdateUI(string fileId)
        {
            await Clients.All.SendAsync("FileDeleted", fileId);
        }
    }
}
