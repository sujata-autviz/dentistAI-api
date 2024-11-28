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

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new { Success = true, Message = "Users retrieved successfully.", Data = users });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving users.", Error = ex.Message });
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(objectId);
                if (user == null) return NotFound(new { Success = false, Message = "User not found." });
                return Ok(new { Success = true, Message = "User retrieved successfully.", Data = user });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving the user.", Error = ex.Message });
            }
        }

        [HttpGet("GetUsersByRole/{role}")]
        public async Task<ActionResult<List<User>>> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(new { Success = true, Message = "Users retrieved successfully.", Data = users });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving users by role.", Error = ex.Message });
            }
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(User user)
        {
            if (!string.IsNullOrEmpty(user.Role))
            {
                user.Role = user.Role.ToUpper();
            }

            // Validate TenantId if it's supposed to be an ObjectId
            if (!string.IsNullOrEmpty(user.TenantId) && !ObjectId.TryParse(user.TenantId, out _))
            {
                return BadRequest(new { Success = false, Message = "Invalid TenantId format." });
            }

            try
            {
                await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { Success = true, Message = "User created successfully.", Data = user });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while creating the user.", Error = ex.Message });
            }
        }
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult> UpdateUser(string id, User user)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            user.Id = objectId;

            try
            {
                bool updateSuccess = await _userService.UpdateUserAsync(objectId, user);
                if (updateSuccess)
                {
                    return Ok(new { Success = true, Message = "User updated successfully." }); // Return success message
                }
                else
                {
                    return NotFound(new { Success = false, Message = "User not found or not modified." }); // Return not found if no user was updated
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while updating the user.", Error = ex.Message });
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            try
            {
                await _userService.SoftDeleteUserAsync(objectId); // Use soft delete
                return NoContent(); // No content for successful delete
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while deleting the user.", Error = ex.Message });
            }
        }
    }
}