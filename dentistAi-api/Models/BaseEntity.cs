using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dentistAi_api.Models
{
    public class BaseEntity
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false; 
    }
}
