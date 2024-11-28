using dentistAi_api.Models;

namespace dentistAi_api.Services
{
    public interface IRoleService
    {
        Task<Role> GetRoleByIdAsync(string id); // Retrieve a role by ID
        Task<IEnumerable<Role>> GetRolesByTenantIdAsync(string tenantId); // Retrieve roles by tenant ID
        Task<bool> AddRoleAsync(Role role); // Add a new role
        Task<bool> UpdateRoleAsync(string id, Role role); // Update an existing role
        Task<bool> DeleteRoleAsync(string id);
    }
}
