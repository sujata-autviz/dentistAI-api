using dentistAi_api.Models;
using MongoDB.Bson;

namespace dentistAi_api.Services
{
    public interface IUserService
    {
        // Get all users for a specific tenant
        Task<List<User>> GetAllUsersAsync(string tenantId);

        // Get a user by their unique ID for a specific tenant
        Task<User> GetUserByIdAsync(ObjectId id, string tenantId);

        // Get users by their role, scoped by tenant
        Task<List<User>> GetUsersByRoleAsync(string role, string tenantId);

        // Create a new user for a specific tenant
        Task CreateUserAsync(User user, string tenantId);

        // Update an existing user’s information for a specific tenant
        Task<bool> UpdateUserAsync(ObjectId id, User user, string tenantId);

        // Soft delete a user for a specific tenant
        Task SoftDeleteUserAsync(ObjectId id, string tenantId);
    }
}
