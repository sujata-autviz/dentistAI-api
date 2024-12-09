using dentistAi_api.DTOs;
using dentistAi_api.Models;
using static dentistAi_api.Services.PeriodontalChartService;

namespace dentistAi_api.Services
{
    public interface IPeriodontalChartService
    {
        Task<IEnumerable<PeriodontalChartDto>> GetChartsByPatientAndTenantIdAsync(string patientId, string tenantId);
        Task<PeriodontalChartDto> GetChartByIdAsync(string id, string tenantId); // Retrieve a chart by ID
        Task<IEnumerable<PeriodontalChartDto>> GetChartsByPatientIdAsync(string patientId); // Retrieve charts by patient ID
        Task<string> AddChartAsync(PeriodontalChart chart); // Add a new chart
        Task<bool> UpdateChartAsync(string id, PeriodontalChart chart); // Update an existing chart
        Task<bool> DeleteChartAsync(string id, string tenantId); // Soft delete a chart
        Task<(bool success, string? chartId)> AddOrUpdateTeethAsync(string? chartId,string tenantId, string patientId,string doctorId, List<Tooth> teeth); // Add or update teeth in a chart
    }
}
