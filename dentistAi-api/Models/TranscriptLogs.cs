using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{

    [BsonCollection("TranscriptLogs")]
    public class TranscriptLogs : BaseEntity
    {
        public string Text { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ChartId { get; set; }
     
    }
}
