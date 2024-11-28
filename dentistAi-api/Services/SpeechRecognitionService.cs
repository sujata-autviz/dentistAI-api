using dentistAi_api.DTOs;
using dentistAi_api.Models;
using Newtonsoft.Json;

namespace dentistAi_api.Services
{
    public class SpeechRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly TranscriptLogsService _transcriptService;
        public SpeechRecognitionService(HttpClient httpClient, IConfiguration config, TranscriptLogsService transcriptService)
        {
            _httpClient = httpClient;
            _config = config;
            _transcriptService = transcriptService;
        }

        public async Task<string> StartVoiceRecognitionAsync(VoiceRecognitionRequestDto request)
        {
            var url = $"{_config["ApiSettings:PythonApiUrl"]}/speech-to-text";

            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SpeechRecognitionResponseDto>(jsonResponse);
                var transcriptLog = new TranscriptLogs
                {
                    Text = result.Text != null? result.Text.ToLower():"" ,
                    TenantId = request.TenantId,
                    ChartId = request.ChartId,
                    // Set other properties as needed, e.g., TenantId, ChartId
                };

                // Add the transcript to the database
                await _transcriptService.AddTranscriptLogAsync(transcriptLog);

                return result?.Text.ToLower() ?? throw new InvalidOperationException("No text found in the response.");
            }

            throw new HttpRequestException("Failed to start voice recognition.");
        }

    }

}

