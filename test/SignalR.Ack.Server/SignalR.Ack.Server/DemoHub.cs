using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalR.Ack.Server
{

    public interface IDemoClient
    {
        Task ReceiveMessage(string message);
    }

    public class DemoHub : Hub<IDemoClient>
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
            return Clients.All.ReceiveMessage($"Echo {message}");
        }
    }
}
