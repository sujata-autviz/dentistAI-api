using System.Text.Json.Serialization;

namespace dentistAi_api.DTOs
{
    public class SpeechRecognitionResponseDto
    {
        public string Status { get; set; } // Maps to "status" in the JSON response
        public string Text { get; set; }   // Maps to "text" in the JSON response

        // Optional property to rename "Text" to "Transcript" for clarity
        [JsonIgnore]
        public string Transcript => Text;
    }
}
