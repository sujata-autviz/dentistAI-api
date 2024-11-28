using dentistAi_api.Models;

namespace dentistAi_api.DTOs
{
    public class PeriodontalChartDto : BaseEntityDto
    {
        public string PatientID { get; set; }
        public string TenantId { get; set; }
        public DateTime ChartDate { get; set; } // Include ChartDate if needed
        public bool IsDeleted { get; set; }
        public List<Tooth> Teeth { get; set; } = new List<Tooth>();// Include IsDeleted if needed
                                                                   // Add other properties as needed
    }
}
