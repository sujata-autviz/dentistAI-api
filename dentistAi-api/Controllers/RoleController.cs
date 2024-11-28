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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var role = await _roleService.GetRoleByIdAsync(objectId.ToString());
            if (role == null)
            {
                return NotFound(new { Success = false, Message = "Role not found." });
            }
            return Ok(new { Success = true, Message = "Role retrieved successfully.", Role = role });
        }

        [HttpGet("GetRolesByTenantId/{tenantId}")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRolesByTenantId(string tenantId)
        {
            var roles = await _roleService.GetRolesByTenantIdAsync(tenantId);
            return Ok(new { Success = true, Message = "Roles retrieved successfully.", Roles = roles });
        }

        [HttpPost("AddRole")]
        public async Task<ActionResult> AddRole([FromBody] Role role)
        {
            await _roleService.AddRoleAsync(role);
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new { Success = true, Message = "Role added successfully.", Role = role });
        }

        [HttpPut("UpdateRole/{id}")]
        public async Task<ActionResult> UpdateRole(string id, [FromBody] Role role)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _roleService.UpdateRoleAsync(objectId.ToString(), role);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Role not found." });
            }
            return Ok(new { Success = true, Message = "Role updated successfully." });
        }

        [HttpDelete("DeleteRole/{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _roleService.DeleteRoleAsync(objectId.ToString());
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Role not found." });
            }
            return Ok(new { Success = true, Message = "Role soft deleted successfully." });
        }
    }
}