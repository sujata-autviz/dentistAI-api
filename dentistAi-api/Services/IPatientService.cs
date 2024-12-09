using dentistAi_api.DTOs;
using dentistAi_api.Models;

namespace dentistAi_api.Services
{
    public interface IPatientService
    {
        Task<Patient> GetPatientByIdAsync(string id , string tenantId); // Retrieve a patient by ID
        Task<IEnumerable<Patient>> GetPatientsByTenantIdAsync(string tenantId); // Retrieve patients by tenant ID
        Task<PatientServiceResponse> AddPatientAsync(PatientDto patient); // Add a new patient
        Task<bool> UpdatePatientAsync(string id, PatientDto patient); // Update an existing patient
        Task<bool> DeletePatientAsync(string id , string tenentId);
        Task<int> GetNextPatientIdAsync(string tenantId, string doctorId);

        Task<Patient> GetPatientByPatientIdAsync(int? patientId, string tenantId);

        // Method for retrieving patients by Doctor IDs with pagination
        Task<PaginatedResult<PatientDto>> GetPatientsByDoctorIdsWithPaginationAsync(
            IEnumerable<string> doctorIds,
            string tenantId,
            int pageNumber,
            int pageSize,
            string searchTerm
        );

    }
}
