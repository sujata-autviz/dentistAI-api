using dentistAi_api.Data;
using dentistAi_api.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace dentistAi_api.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbContext context)
        {
            _users = context.Users; // Access the Users collection from the context
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            // Fetch only non-deleted users
            return await _users.Find(user => !user.IsDeleted).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(ObjectId id)
        {
            return await _users.Find(user => user.Id == id && !user.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            // Fetch users by role, excluding deleted users
            return await _users.Find(user => user.Role == role && !user.IsDeleted).ToListAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            await _users.InsertOneAsync(user);
        }

        public async Task<bool> UpdateUserAsync(ObjectId id, User user)
        {
            user.Id = id; // Ensure the user ID is set correctly
            var result = await _users.ReplaceOneAsync(u => u.Id == id && !u.IsDeleted, user);
            return result.ModifiedCount > 0; // Return true if a document was modified
        }

        public async Task SoftDeleteUserAsync(ObjectId id)
        {
            var update = Builders<User>.Update.Set(u => u.IsDeleted, true);
            await _users.UpdateOneAsync(u => u.Id == id, update);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }

}