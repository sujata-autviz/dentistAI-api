using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace dentistAi_api.NotificationHub
{
    public class NotificationsHub : Hub
    {
        // Store user connections in a static dictionary
        private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

        // This is called when a client connects to the SignalR hub
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated ?? false)
            {
                var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine($"User {userId} connected.");
                }
                else
                {
                    Console.WriteLine("User ID claim is missing.");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated.");
            }

            await base.OnConnectedAsync();
        }


        // This is called when a client disconnects
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Extract userId from JWT token again
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && _connections.ContainsKey(userId))
            {
                // Remove the connection ID for the user
                _connections.Remove(userId);
                Console.WriteLine($"User {userId} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // This method sends a notification to a specific user
        public async Task SendNotification(string userId, object message)
        {
            if (_connections.ContainsKey(userId))
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
