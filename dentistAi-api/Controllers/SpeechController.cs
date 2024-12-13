using dentistAi_api.DTOs;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp.Unsafe;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeechController : ControllerBase
    {
        private readonly SpeechRecognitionService _speechRecognitionService;

        public SpeechController(SpeechRecognitionService speechRecognitionService)
        {
            _speechRecognitionService = speechRecognitionService;
        }

        [HttpPost("Start")]
        public async Task<IActionResult> StartVoiceRecognition([FromBody] VoiceRecognitionRequestDto request)
        {
            try
            {
                var transcript = await _speechRecognitionService.StartVoiceRecognitionAsync(request);
                return Ok(new { Success = true, Transcript = transcript });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("StartVoiceRecognition")]
        public async Task<IActionResult> StartVoiceRecognitionAsync(VoiceRecognitionRequestDto request)
        {
            if (request.AudioFile == null || request.AudioFile.Length == 0)
            {
                return BadRequest("Audio file is required.");
            }

            var result = await _speechRecognitionService.ProcessAudioAsync(request);
            return Ok(new { content = result });
        }

      

    }
}