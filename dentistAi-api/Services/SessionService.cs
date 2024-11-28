using dentistAi_api.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace dentistAi_api.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MongoDbContext _context;

        public SessionService(IHttpContextAccessor httpContextAccessor, MongoDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // Get the current user details
        public async Task<(string Id, string FirstName, string LastName, string Email, string UserName, string ProfileImage, string TenantId)> GetCurrentUserAsync()
        {
            var userIdString = GetCurrentUserId(); // Get the user ID as a string
            if (userIdString == null)
            {
                return (null, null, null, null, null, null,null);
            }

            // Convert the string userId to ObjectId
            if (!ObjectId.TryParse(userIdString, out var userId))
            {
                return (null, null, null, null, null, null , null); // Return null if parsing fails
            }

            var user = await _context.Users
                .Find(u => u.Id == userId && !u.IsDeleted) // Use Find for MongoDB
                .FirstOrDefaultAsync();

            return user == null ? (null, null, null, null, null, null , null) : (user.Id.ToString(), user.FirstName, user.LastName, user.Email, user.UserName, user.ProfileImage , user.TenantId);
        }

        // Get the current user ID from claims
        public string GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value; // Return the user ID as a string
        }

        // Check if the user is authenticated
        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }

        // Get the roles of the current user
        public IEnumerable<string> GetUserRoles()
        {
            return _httpContextAccessor.HttpContext?.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }
}
