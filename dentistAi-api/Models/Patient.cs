using dentistAi_api.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dentistAi_api.Models
{
    [BsonCollection("patients")]
    public class Patient : BaseEntity
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; } // Identifier for multi-tenancy

        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

}
