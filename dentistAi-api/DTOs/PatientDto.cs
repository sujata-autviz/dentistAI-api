namespace dentistAi_api.DTOs
{
    public class PatientDto
    {
        public string? Id { get; set; }
        public string TenantId { get; set; }
        public int? PatientId { get; set; }
        public string DoctorId { get; set; } 
        public string Name { get; set; } 
        public int Age { get; set; } 
        public string Phone { get; set; } 
        public DateTime DateOfBirth { get; set; } 
        public bool IsDeleted { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
    }
}
