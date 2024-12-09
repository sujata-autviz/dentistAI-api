using dentistAi_api.Models;

namespace dentistAi_api.DTOs
{
    public class AddOrUpdateTeethDto
    {
        public string TenantId { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string? ChartId { get; set; }
        public List<Tooth> Teeth { get; set; } 
    }
}
