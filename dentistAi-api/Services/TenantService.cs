using dentistAi_api.Data;
using dentistAi_api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dentistAi_api.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMongoCollection<Tenant> _tenants;

        public TenantService(MongoDbContext context)
        {
            _tenants = context.Tenants; // Access the Tenants collection from the context
        }

        public async Task<List<Tenant>> GetAllTenantsAsync()
        {
            return await _tenants.Find(tenant => true).ToListAsync();
        }

        public async Task<Tenant> GetTenantByIdAsync(ObjectId id)
        {
            return await _tenants.Find(tenant => tenant.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateTenantAsync(Tenant tenant)
        {
            await _tenants.InsertOneAsync(tenant);
        }

        public async Task<bool> UpdateTenantAsync(ObjectId id, Tenant tenant)
        {
            var result = await _tenants.ReplaceOneAsync(t => t.Id == id, tenant);
            return result.ModifiedCount > 0; 
        }

        public async Task DeleteTenantAsync(ObjectId id)
        {
            await _tenants.DeleteOneAsync(t => t.Id == id);
        }
    }
}
