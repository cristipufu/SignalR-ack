using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalR.Ack.Server
{
    public class DemoHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Topic");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Topic");

            await base.OnDisconnectedAsync(exception);
        }

        public Task SendMessage(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", $"Echo {message}");
        }
    }
}
