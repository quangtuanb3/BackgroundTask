using Microsoft.AspNetCore.SignalR;
namespace MergeWeb.Hubs;

public class UploadHub : Hub
{
    private static readonly Dictionary<string, string> SessionConnections = new Dictionary<string, string>();
    public async Task SendMessage(string message)
    {
        // Log or process the received message
        Console.WriteLine($"Received message: {message}");
        // Optionally, send the message back to all clients
        await Clients.Caller.SendAsync("ReceiveMessage", message);
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        Console.WriteLine($"Received message: {message}");

        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }

    public async Task RegisterConnection(string connectionId, string sessionId)
    {
        SessionConnections[sessionId] = connectionId;
    }

    public async Task SendMessageToSession(string sessionId, string message)
    {
        if (SessionConnections.TryGetValue(sessionId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var sessionId = SessionConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (sessionId != null)
        {
            SessionConnections.Remove(sessionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
