using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{
    [BsonCollection("periodontal_charts")]
    public class PeriodontalChart : BaseEntity
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; } // Identifier for multi-tenancy

        public string PatientID { get; set; } // Reference to the patient
        public string DoctorId { get; set; } // Reference to the patient
        public DateTime ChartDate { get; set; } // Date of chart creation

        // Embedded list of teeth details
        public List<Tooth> Teeth { get; set; } = new List<Tooth>();
    }
}
