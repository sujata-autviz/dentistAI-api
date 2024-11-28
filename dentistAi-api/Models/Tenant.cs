using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{
    [BsonCollection("tenants")]
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } // Name of the tenant
        public string Email { get; set; } // Contact email
        public string SubscriptionPlan { get; set; } // E.g., "Basic", "Premium"
    }
}
