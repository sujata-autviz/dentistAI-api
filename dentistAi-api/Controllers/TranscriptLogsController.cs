using dentistAi_api.Models;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptLogsController : ControllerBase
    {
        private readonly ITranscriptLogsService _transcriptLogsService;

        public TranscriptLogsController(ITranscriptLogsService transcriptLogsService)
        {
            _transcriptLogsService = transcriptLogsService;
        }

        [HttpPost("AddTranscriptLogs")]
        public async Task<IActionResult> AddTranscriptLog([FromBody] TranscriptLogs transcriptLog)
        {
            if (transcriptLog == null || string.IsNullOrWhiteSpace(transcriptLog.Text))
            {
                return BadRequest("Transcript log cannot be empty.");
            }

            await _transcriptLogsService.AddTranscriptLogAsync(transcriptLog);
            return Ok(new { Success = true, Message = "Transcript log added successfully." });
        }

        [HttpGet("GetAllTranscriptLogs")]
        public async Task<ActionResult<IEnumerable<TranscriptLogs>>> GetAllTranscriptLogs()
        {
            var logs = await _transcriptLogsService.GetAllTranscriptLogsAsync();
            return Ok(logs);
        }

        [HttpGet("GetTranscriptLogById/{id}")]
        public async Task<ActionResult<TranscriptLogs>> GetTranscriptLogById(string id)
        {
            var log = await _transcriptLogsService.GetTranscriptLogByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            return Ok(log);
        }
    }
}