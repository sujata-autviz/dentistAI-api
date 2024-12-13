using dentistAi_api.DTOs;
using dentistAi_api.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static dentistAi_api.Services.SpeechRecognitionService;

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

        //public async Task<string> StartVoiceRecognitionAsync(VoiceRecognitionRequestDto request)
        //{
        //    var url = $"{_config["ApiSettings:PythonApiUrl"]}/speech-to-text";

        //    var response = await _httpClient.PostAsync(url, null);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var jsonResponse = await response.Content.ReadAsStringAsync();
        //        var result = JsonConvert.DeserializeObject<SpeechRecognitionResponseDto>(jsonResponse);
        //        var transcriptLog = new TranscriptLogs
        //        {
        //            Text = result.Text != null? result.Text.ToLower():"" ,
        //            TenantId = request.TenantId,
        //            ChartId = request.ChartId,
        //            // Set other properties as needed, e.g., TenantId, ChartId
        //        };

        //        // Add the transcript to the database
        //        await _transcriptService.AddTranscriptLogAsync(transcriptLog);

        //        return result?.Text.ToLower() ?? throw new InvalidOperationException("No text found in the response.");
        //    }

        //    throw new HttpRequestException("Failed to start voice recognition.");
        //}

        public async Task<string> StartVoiceRecognitionAsync(VoiceRecognitionRequestDto request)
        {
            var url = $"{_config["ApiSettings:PythonApiUrl"]}/speech-to-text";

            using (var content = new MultipartFormDataContent())
            {
                // Add patientId, doctorId, chartId as string content
                content.Add(new StringContent(request.PatientId.ToString()), "patientId");
                content.Add(new StringContent(request.DoctorId.ToString()), "doctorId");
                content.Add(new StringContent(request.ChartId.ToString()), "chartId");

                // Add the audio file (ensure request.AudioFile is a valid file stream or byte array)
                if (request.AudioFile != null && request.AudioFile.Length > 0)
                {
                    // Use OpenReadStream to get a valid stream from IFormFile
                    var fileContent = new StreamContent(request.AudioFile.OpenReadStream()); // Use OpenReadStream()
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg"); // Set appropriate content type for your audio file
                    content.Add(fileContent, "audio", "audio_file_name.mp3"); // Provide a valid file name
                }

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SpeechRecognitionResponseDto>(jsonResponse);
                    var transcriptLog = new TranscriptLogs
                    {
                        Text = result.Text != null ? result.Text.ToLower() : "",
                        TenantId = request.TenantId,
                        ChartId = request.ChartId,
                        // Set other properties as needed
                    };

                    // Add the transcript to the database
                    await _transcriptService.AddTranscriptLogAsync(transcriptLog);

                    return result?.Text.ToLower() ?? throw new InvalidOperationException("No text found in the response.");
                }
            }

            throw new HttpRequestException("Failed to start voice recognition.");
        }


        public async Task<string> ProcessAudioAsync(VoiceRecognitionRequestDto request)
        {
            var url = $"{_config["ApiSettings:PythonApiUrl"]}/process-audio/";

            using (var content = new MultipartFormDataContent())
            {
                // Add userID and patientID as string content
                content.Add(new StringContent(request.DoctorId), "userID");
                content.Add(new StringContent(request.PatientId), "patientID");

                // Add the audio file
                if (request.AudioFile != null && request.AudioFile.Length > 0)
                {
                    var fileContent = new StreamContent(request.AudioFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                    content.Add(fileContent, "file", request.AudioFile.FileName);
                }

                // Make the HTTP POST request
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse; // Return the API response
                }
            }

            throw new HttpRequestException("Failed to process audio.");
        }

    }

}

