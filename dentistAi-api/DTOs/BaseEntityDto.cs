using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dentistAi_api.DTOs
{
    public class BaseEntityDto
    {

        public string Id { get; set; } 

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 

        public bool IsDeleted { get; set; } 
    }
}
