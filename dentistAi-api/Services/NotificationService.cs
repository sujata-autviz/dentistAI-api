using dentistAi_api.NotificationHub;
using Microsoft.AspNetCore.SignalR;

namespace dentistAi_api.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public NotificationService(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
          //  await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

    }

}

