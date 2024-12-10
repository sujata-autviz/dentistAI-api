using dentistAi_api.Data;
using dentistAi_api.Models;
using dentistAi_api.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Role> _roles;

    public UserService(MongoDbContext context)
    {
        _users = context.Users;
        _roles = context.Roles;
    }

    public async Task<List<User>> GetAllUsersAsync(string tenantId)
    {
        return await _users.Find(user => user.TenantId == tenantId && !user.IsDeleted).ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(ObjectId id, string tenantId)
    {
        return await _users.Find(user => user.Id == id && user.TenantId == tenantId && !user.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetUsersByRoleAsync(string role, string tenantId)
    {
        return await _users.Find(user => user.Role == role && user.TenantId == tenantId && !user.IsDeleted).ToListAsync();
    }

    public async Task CreateUserAsync(User user, string tenantId)
    {
        // Ensure the user has the correct tenantId
        user.TenantId = tenantId;
        user.PasswordHash = HashPassword(user.PasswordHash);
        var existingRole = await _roles.Find(r => r.Name == user.Role && r.TenantId == tenantId && !r.IsDeleted).FirstOrDefaultAsync();

        // If the role doesn't exist, create it
        if (existingRole == null)
        {
            var newRole = new Role
            {
                Name = user.Role,
                TenantId = tenantId,
                IsDeleted = false
            };

            // Insert the new role into the Roles collection
            await _roles.InsertOneAsync(newRole);

            // Optionally, assign the new role to the user
            user.Role = newRole.Name;
        }
        await _users.InsertOneAsync(user);
    }

    public async Task<bool> UpdateUserAsync(ObjectId id, User user, string tenantId)
    {
        user.Id = id; // Ensure the user ID is set correctly

        // Check if the role exists in the Roles collection
        var existingRole = await _roles.Find(r => r.Name == user.Role && r.TenantId == tenantId && !r.IsDeleted).FirstOrDefaultAsync();

        // If the role doesn't exist, create it
        if (existingRole == null)
        {
            var newRole = new Role
            {
                Name = user.Role,
                TenantId = tenantId,
                IsDeleted = false
            };

            // Insert the new role into the Roles collection
            await _roles.InsertOneAsync(newRole);

            // Optionally, assign the new role to the user
            user.Role = newRole.Name;
        }

        // Perform the user update in the Users collection
        var result = await _users.ReplaceOneAsync(u => u.Id == id && u.TenantId == tenantId && !u.IsDeleted, user);
        return result.ModifiedCount > 0; // Return true if a document was modified
    }

    public async Task SoftDeleteUserAsync(ObjectId id, string tenantId)
    {
        var update = Builders<User>.Update.Set(u => u.IsDeleted, true);
        await _users.UpdateOneAsync(u => u.Id == id && u.TenantId == tenantId, update);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
