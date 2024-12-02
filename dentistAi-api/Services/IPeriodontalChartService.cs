using dentistAi_api.DTOs;
using dentistAi_api.Models;
using static dentistAi_api.Services.PeriodontalChartService;

namespace dentistAi_api.Services
{
    public interface IPeriodontalChartService
    {
        Task<PeriodontalChart> GetChartByIdAsync(string id); // Retrieve a chart by ID
        Task<IEnumerable<PeriodontalChartDto>> GetChartsByPatientIdAsync(string patientId); // Retrieve charts by patient ID
        Task<bool> AddChartAsync(PeriodontalChart chart); // Add a new chart
        Task<bool> UpdateChartAsync(string id, PeriodontalChart chart); // Update an existing chart
        Task<bool> DeleteChartAsync(string id); // Soft delete a chart
        Task<bool> AddOrUpdateTeethAsync(string? chartId,string tenantId, string patientId, List<Tooth> teeth); // Add or update teeth in a chart
    }
}
