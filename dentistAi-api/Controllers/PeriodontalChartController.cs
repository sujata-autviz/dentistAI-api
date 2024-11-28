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

        [HttpGet("GetChart/{id}")]
        public async Task<ActionResult<PeriodontalChart>> GetChart(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var chart = await _chartService.GetChartByIdAsync(objectId.ToString());
            if (chart == null)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Chart retrieved successfully.", Chart = chart });
        }

        [HttpGet("GetChartsByPatientId/{patientId}")]
        public async Task<ActionResult<IEnumerable<PeriodontalChartDto>>> GetChartsByPatientId(string patientId)
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
        public async Task<ActionResult> DeleteChart(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _chartService.DeleteChartAsync(objectId.ToString());
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Chart soft deleted successfully." });
        }

        [HttpPost("AddOrUpdateTeeth/{patientId}")]
        public async Task<ActionResult> AddOrUpdateTeeth(string patientId, [FromBody] List<Tooth> teeth)
        {
            if (teeth == null || !teeth.Any())
            {
                return BadRequest(new { Success = false, Message = "Teeth list cannot be empty." });
            }

            // Optionally, you can pass a chartId if you want to update an existing chart
            string chartId = teeth.First().ChartId; // Assuming the first tooth has the ChartId
            var success = await _chartService.AddOrUpdateTeethAsync(chartId, patientId, teeth);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chart not found." });
            }
            return Ok(new { Success = true, Message = "Teeth added or updated successfully." });
        }
    }
}