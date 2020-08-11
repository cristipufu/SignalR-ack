using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Ack.Client
{
    class Program
    {
        static HubConnection connection;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44343/DemoHub")
                .WithAutomaticReconnect()
                .Build();

            connection.Reconnecting += error =>
            {
                Debug.Assert(connection.State == HubConnectionState.Reconnecting);
                Console.WriteLine("Reconnecting...");

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.

                return Task.CompletedTask;
            };

            connection.Reconnected += connectionId =>
            {
                Debug.Assert(connection.State == HubConnectionState.Connected);
                Console.WriteLine("Reconnected.");

                // Notify users the connection was reestablished.
                // Start dequeuing messages queued while reconnecting if any.

                return Task.CompletedTask;
            };

            connection.Closed += error =>
            {
                Debug.Assert(connection.State == HubConnectionState.Disconnected);
                Console.WriteLine("Disconnected.");

                // Notify users the connection has been closed or manually try to restart the connection.

                return Task.CompletedTask;
            };

            connection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"Received message: {message}");
            });

            await ConnectWithRetryAsync(connection, CancellationToken.None);

            var i = 0;

            while (true)
            {
                await connection.InvokeAsync("SendMessage", $"message-{i++}");

                await Task.Delay(5000);
            }
        }

        public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    Console.WriteLine("Connected.");

                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    // Failed to connect, trying again in 5000 ms.
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    Console.WriteLine("Failed to connect, trying again in 5000 ms.");

                    await Task.Delay(5000);
                }
            }
        }
    }
}
