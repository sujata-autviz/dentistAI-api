using dentistAi_api.DTOs;
using dentistAi_api.Models;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using static dentistAi_api.Services.PeriodontalChartService;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeriodontalChartController : ControllerBase
    {
        private readonly IPeriodontalChartService _chartService;

        public PeriodontalChartController(IPeriodontalChartService chartService)
        {
            _chartService = chartService;
        }

        [HttpGet("GetChart/{id}/{tenantId}")]
        public async Task<ActionResult<PeriodontalChartDto>> GetChart(string id, string tenantId)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var chart = await _chartService.GetChartByIdAsync(objectId.ToString(), tenantId);
            if (chart == null)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Chart retrieved successfully.", Chart = chart });
        }

        [HttpGet("GetChartsByPatientId/{patientId}")]
        public async Task<ActionResult<IEnumerable<PeriodontalChartDto>>> GetChartsByPatientId(string patientId ,string tenantId)
        {
            var charts = await _chartService.GetChartsByPatientIdAsync(patientId);
            return Ok(new { Success = true, Message = "Charts retrieved successfully.", Charts = charts });
        }

        [HttpPost("AddChart")]
        public async Task<ActionResult> AddChart([FromBody] PeriodontalChart chart)
        {
            await _chartService.AddChartAsync(chart);
            return CreatedAtAction(nameof(GetChart), new { id = chart.Id }, new { Success = true, Message = "Chart added successfully.", Chart = chart });
        }

        [HttpPut("UpdateChart/{id}")]
        public async Task<ActionResult> UpdateChart(string id, [FromBody] PeriodontalChart chart)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _chartService.UpdateChartAsync(objectId.ToString(), chart);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Chart updated successfully." });
        }

        [HttpDelete("DeleteChart/{id}")]
        public async Task<ActionResult> DeleteChart(string id , string tenantId)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _chartService.DeleteChartAsync(objectId.ToString(), tenantId);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Chart soft deleted successfully." });
        }

       

        [HttpPost("AddOrUpdateTeeth")]
        public async Task<ActionResult> AddOrUpdateTeeth([FromBody] AddOrUpdateTeethDto input)
        {
            if (input.Teeth == null || !input.Teeth.Any())
            {
                return BadRequest(new { Success = false, Message = "Teeth list cannot be empty." });
            }

            var (success, chartId) = await _chartService.AddOrUpdateTeethAsync(input.ChartId, input.TenantId, input.PatientId, input.DoctorId, input.Teeth);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Teeth added or updated successfully." , chartId = chartId });
        }

        [HttpGet("GetChartsByPatientAndTenantId")]
        public async Task<ActionResult> GetChartsByPatientAndTenantId(
     [FromQuery] string patientId,
     [FromQuery] string tenantId)
        {
            if (string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(tenantId))
            {
                return BadRequest(new { Success = false, Message = "PatientId and TenantId are required." });
            }

            var charts = await _chartService.GetChartsByPatientAndTenantIdAsync(patientId, tenantId);

            if (charts == null || !charts.Any())
            {
                return Ok(new { Success = false, Message = "No charts found for the given PatientId and TenantId." });
            }

            return Ok(new { Success = true, Message = "Charts retrieved successfully.", Chart = charts });
        }


    }
}