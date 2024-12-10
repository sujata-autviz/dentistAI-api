using dentistAi_api.Models;

namespace dentistAi_api.Services
{
    public interface IToothService
    {
        Task<Tooth> GetToothByIdAsync(string id); // Retrieve a tooth by ID
        //Task<IEnumerable<Tooth>> GetTeethByChartIdAsync(string chartId); // Retrieve teeth by chart ID
        Task<bool> AddToothAsync(Tooth tooth); // Add a new tooth
        Task<bool> UpdateToothAsync(string id, Tooth tooth); // Update an existing tooth
        Task<bool> DeleteToothAsync(string id);
    }
}
