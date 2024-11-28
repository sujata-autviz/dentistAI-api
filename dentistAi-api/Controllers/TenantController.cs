using dentistAi_api.Models;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("GetAllTenants")]
        public async Task<ActionResult<List<Tenant>>> GetAllTenants()
        {
            try
            {
                var tenants = await _tenantService.GetAllTenantsAsync();
                return Ok(new { Success = true, Message = "Tenants retrieved successfully.", Data = tenants });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving tenants.", Error = ex.Message });
            }
        }

        [HttpGet("GetTenantById/{id}")]
        public async Task<ActionResult<Tenant>> GetTenantById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            try
            {
                var tenant = await _tenantService.GetTenantByIdAsync(objectId);
                if (tenant == null) return NotFound(new { Success = false, Message = "Tenant not found." });
                return Ok(new { Success = true, Message = "Tenant retrieved successfully.", Data = tenant });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while retrieving the tenant.", Error = ex.Message });
            }
        }

        [HttpPost("CreateTenant")]
        public async Task<ActionResult> CreateTenant(Tenant tenant)
        {
            try
            {
                await _tenantService.CreateTenantAsync(tenant);
                return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, new { Success = true, Message = "Tenant created successfully.", Data = tenant });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while creating the tenant.", Error = ex.Message });
            }
        }
        [HttpPut("UpdateTenant/{id}")]
        public async Task<ActionResult> UpdateTenant(string id, Tenant tenant)
        {
            // Validate the ID format
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            // Set the Id to ensure it matches the one being updated
            tenant.Id = objectId;

            try
            {
                // Call the service to update the tenant
                bool updateSuccess = await _tenantService.UpdateTenantAsync(objectId, tenant);
                if (updateSuccess)
                {
                    return Ok(new { Success = true, Message = "Tenant updated successfully." }); // Return success message
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Tenant not found or not modified." }); // Return not found if no tenant was updated
                }
            }
            catch (MongoWriteException mongoEx)
            {
                // Log the error details for debugging
                Console.WriteLine($"MongoDB Write Error: {mongoEx.Message}");
                return StatusCode(500, new { Success = false, Message = "An error occurred while updating the tenant.", Error = mongoEx.Message });
            }
            catch (Exception ex)
            {
                // Log the general exception
                Console.WriteLine($"General Error: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpDelete("DeleteTenant/{id}")]
        public async Task<ActionResult> DeleteTenant(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            try
            {
                await _tenantService.DeleteTenantAsync(objectId);
                return NoContent(); // No content for successful delete
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Success = false, Message = "An error occurred while deleting the tenant.", Error = ex.Message });
            }
        }
    }
}