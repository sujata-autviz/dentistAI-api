using dentistAi_api.Models;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers/{tenantId}")]
        public async Task<ActionResult<List<User>>> GetAllUsers(string tenantId)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(tenantId);
                return Ok(new { Success = true, Message = "Users retrieved successfully.", Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving users.", Error = ex.Message });
            }
        }

        [HttpGet("GetUserById/{id}/{tenantId}")]
        public async Task<ActionResult<User>> GetUserById(string id, string tenantId)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(objectId, tenantId);
                if (user == null) return NotFound(new { Success = false, Message = "User not found." });
                return Ok(new { Success = true, Message = "User retrieved successfully.", Data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving the user.", Error = ex.Message });
            }
        }

        [HttpGet("GetUsersByRole/{role}/{tenantId}")]
        public async Task<ActionResult<List<User>>> GetUsersByRole(string role, string tenantId)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role, tenantId);
                return Ok(new { Success = true, Message = "Users retrieved successfully.", Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving users by role.", Error = ex.Message });
            }
        }

        [HttpPost("CreateUser/{tenantId}")]
        public async Task<ActionResult> CreateUser(string tenantId, User user)
        {
            try
            {
                await _userService.CreateUserAsync(user, tenantId);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id, tenantId }, new { Success = true, Message = "User created successfully.", Data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while creating the user.", Error = ex.Message });
            }
        }

        [HttpPut("UpdateUser/{id}/{tenantId}")]
        public async Task<ActionResult> UpdateUser(string id, string tenantId, User user)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            user.Id = objectId;

            try
            {
                var updateSuccess = await _userService.UpdateUserAsync(objectId, user, tenantId);
                if (updateSuccess)
                {
                    return Ok(new { Success = true, Message = "User updated successfully." });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "User not found or not modified." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while updating the user.", Error = ex.Message });
            }
        }

        [HttpDelete("DeleteUser/{id}/{tenantId}")]
        public async Task<ActionResult> DeleteUser(string id, string tenantId)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            try
            {
                await _userService.SoftDeleteUserAsync(objectId, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while deleting the user.", Error = ex.Message });
            }
        }
    }
}
