using Microsoft.AspNetCore.SignalR;
namespace MergeWeb.Hubs
{
    public class UploadHub : Hub
    {
        public async Task SendMessage(string message)
        {
            // Log or process the received message
            Console.WriteLine($"Received message: {message}");

            // Optionally, send the message back to all clients
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
