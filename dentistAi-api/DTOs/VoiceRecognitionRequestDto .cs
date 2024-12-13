namespace dentistAi_api.DTOs
{
    public class VoiceRecognitionRequestDto
    {
        public string PatientId { get; set; } // Patient's ID
        public string DoctorId { get; set; }  // Doctor's ID
        public string ChartId { get; set; }   // Chart's ID
        public string TenantId { get; set; }   // Chart's ID
        public IFormFile AudioFile { get; set; } // Uploaded audio file
    }
}
