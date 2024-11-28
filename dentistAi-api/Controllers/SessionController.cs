using dentistAi_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using dentistAi_api.DTOs; // Ensure you have a DTO for User if needed

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _sessionService;

        public SessionController(SessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _sessionService.GetCurrentUserAsync();
            if (user == (null, null, null, null, null, null, null))
            {
                return NotFound(new { Success = false, Message = "User not found." });
            }

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                ProfileImage = user.ProfileImage,
                TenantId = user.TenantId,
               
            };

            return Ok(new { Success = true, Message = "Current user retrieved successfully.", Data = userDto });
        }

        [HttpGet("CurrentUserId")]
        [Authorize]
        public ActionResult<string> GetCurrentUserId()
        {
            var userId = _sessionService.GetCurrentUserId();
            return Ok(new { Success = true, UserId = userId });
        }

        [HttpGet("IsAuthenticated")]
        public ActionResult<bool> IsAuthenticated()
        {
            var isAuthenticated = _sessionService.IsAuthenticated();
            return Ok(new { Success = true, IsAuthenticated = isAuthenticated });
        }

        [HttpGet("UserRoles")]
        [Authorize]
        public ActionResult<IEnumerable<string>> GetUserRoles()
        {
            var roles = _sessionService.GetUserRoles();
            return Ok(new { Success = true, Roles = roles });
        }
    }
}