using dentistAi_api.DTOs;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(NotificationRequestDto notificationRequest)
        {
            await _notificationService.SendNotificationToUser(notificationRequest.UserId, notificationRequest.Message);
            return Ok();
        }

        [HttpPost("SendTranscript")]
        public async Task<IActionResult> SendTranscript(NotificationRequestDto notificationRequest)
        {
            // Ensure the message is parsed correctly as a JObject
            JObject messageJson = JObject.Parse(Convert.ToString(notificationRequest.Message));

           

            // Send the serialized message to the user via SignalR
            await _notificationService.SendNotificationToUser(notificationRequest.UserId, messageJson.ToString());

            return Ok();
        }

    }
}
