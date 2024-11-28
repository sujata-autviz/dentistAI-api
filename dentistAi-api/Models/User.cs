using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{
    [BsonCollection("users")]
    public class User: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } 
        public string UserName { get; set; } 
        public string PasswordHash { get; set; }
        public string? ProfileImage { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; }

        public string Role { get; set; } // Role of the user (e.g., "Admin", "Doctor", "Staff")

        public bool IsActive { get; set; } = true; 

    }
}
