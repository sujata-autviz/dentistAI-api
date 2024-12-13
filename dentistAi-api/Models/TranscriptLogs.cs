using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{

    [BsonCollection("TranscriptLogs")]
    public class TranscriptLogs : BaseEntity
    {
        public string Text { get; set; }
        public string TenantId { get; set; }

        public string ChartId { get; set; }
     
    }
}
