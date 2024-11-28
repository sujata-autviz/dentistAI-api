using dentistAi_api.Data;
using dentistAi_api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dentistAi_api.Services
{
    public class RoleService : IRoleService
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Role> _roles;

        public RoleService(MongoDbContext context)
        {
            _context = context;
            _roles = _context.Roles; // Assuming you have a Roles collection in your MongoDbContext
        }

        public async Task<Role> GetRoleByIdAsync(string id)
        {
            return await _roles.Find(r => r.Id == ObjectId.Parse(id) && !r.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Role>> GetRolesByTenantIdAsync(string tenantId)
        {
            return await _roles.Find(r => r.TenantId == tenantId && !r.IsDeleted).ToListAsync();
        }

        public async Task<bool> AddRoleAsync(Role role)
        {
            await _roles.InsertOneAsync(role);
            return true; // Return true if the role was added successfully
        }

        public async Task<bool> UpdateRoleAsync(string id, Role role)
        {
            var result = await _roles.ReplaceOneAsync(r => r.Id == ObjectId.Parse(id) && !r.IsDeleted, role);
            return result.ModifiedCount > 0; // Return true if a document was modified
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            var role = await GetRoleByIdAsync(id);
            if (role == null)
            {
                return false; // Role not found
            }

            role.IsDeleted = true; // Set the IsDeleted flag to true for soft delete
            await _roles.ReplaceOneAsync(r => r.Id == ObjectId.Parse(id), role); // Update the role
            return true; // Return true if the role was soft deleted
        }
    }
}