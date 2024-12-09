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
    public class ToothController : ControllerBase
    {
        private readonly IToothService _toothService;

        public ToothController(IToothService toothService)
        {
            _toothService = toothService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tooth>> GetTooth(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var tooth = await _toothService.GetToothByIdAsync(objectId.ToString());
            if (tooth == null)
            {
                return NotFound(new { Success = false, Message = "Tooth not found." });
            }
            return Ok(new { Success = true, Message = "Tooth retrieved successfully.", Tooth = tooth });
        }

        [HttpGet("GetTeethByChartId/{chartId}")]
        public async Task<ActionResult<IEnumerable<Tooth>>> GetTeethByChartId(string chartId)
        {
            //var teeth = await _toothService.GetTeethByChartIdAsync(chartId);
            var teeth = "";
            return Ok(new { Success = true, Message = "Teeth retrieved successfully.", Teeth = teeth });
        }

        [HttpPost("AddTooth")]
        public async Task<ActionResult> AddTooth([FromBody] Tooth tooth)
        {
            await _toothService.AddToothAsync(tooth);
            return CreatedAtAction(nameof(GetTooth), new { id = tooth.Id }, new { Success = true, Message = "Tooth added successfully.", Tooth = tooth });
        }

        [HttpPut("UpdateTooth/{id}")]
        public async Task<ActionResult> UpdateTooth(string id, [FromBody] Tooth tooth)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _toothService.UpdateToothAsync(objectId.ToString(), tooth);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Tooth not found." });
            }
            return Ok(new { Success = true, Message = "Tooth updated successfully." });
        }

        [HttpDelete("DeleteTooth/{id}")]
        public async Task<ActionResult> DeleteTooth(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _toothService.DeleteToothAsync(objectId.ToString());
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Tooth not found." });
            }
            return Ok(new { Success = true, Message = "Tooth soft deleted successfully." });
        }
    }
}