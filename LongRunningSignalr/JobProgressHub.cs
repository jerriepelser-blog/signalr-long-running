using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace LongRunningSignalr
{
    public class JobProgressHub : Hub
    {
        public async Task AssociateJob(string jobId)
        {
            Context.Items.Add("JobId", jobId);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
        }
        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.Items["JobId"] is string jobId)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}