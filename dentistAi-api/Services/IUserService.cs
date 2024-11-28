using dentistAi_api.Models;
using MongoDB.Bson;

namespace dentistAi_api.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(ObjectId id);
        Task<List<User>> GetUsersByRoleAsync(string role); // New method for getting users by role
        Task CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(ObjectId id, User user);
        Task SoftDeleteUserAsync(ObjectId id);
    }
}
