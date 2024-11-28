using dentistAi_api.Models;
using MongoDB.Bson;

namespace dentistAi_api.Services
{
    public interface ITenantService
    {
        Task<List<Tenant>> GetAllTenantsAsync();
        Task<Tenant> GetTenantByIdAsync(ObjectId id);
        Task CreateTenantAsync(Tenant tenant);
        Task<bool> UpdateTenantAsync(ObjectId id, Tenant tenant);
        Task DeleteTenantAsync(ObjectId id);
    }
}
