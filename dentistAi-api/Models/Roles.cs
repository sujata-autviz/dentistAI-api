using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{
    [BsonCollection("roles")]
    public class Role : BaseEntity
    {


        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; } // Reference to the tenant

        public string Name { get; set; } // Role name (e.g., "Admin", "Doctor")
        public List<string> Permissions { get; set; } = new List<string>(); // E.g., ["Read", "Write", "Delete"]
    }
}
