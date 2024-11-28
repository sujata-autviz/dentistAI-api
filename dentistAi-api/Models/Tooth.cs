using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using dentistAi_api.Attributes;

namespace dentistAi_api.Models
{
    [BsonCollection("teeth")]
    public class Tooth : BaseEntity
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; } // Identifier for multi-tenancy

        [BsonRepresentation(BsonType.ObjectId)]
        public string ChartId { get; set; } // Reference to the periodontal chart

        public int ToothNumber { get; set; } // E.g., 19, 32

        // Pocket Depth measurements (PD) - 6 points per tooth
        public int? PocketDepthBuccalLeft { get; set; }
        public int? PocketDepthBuccalCenter { get; set; }
        public int? PocketDepthBuccalRight { get; set; }
        public int? PocketDepthLingualLeft { get; set; }
        public int? PocketDepthLingualCenter { get; set; }
        public int? PocketDepthLingualRight { get; set; }

        // Gingival Margin measurements (GM) - 6 points per tooth
        public int? GingivalMarginBuccalLeft { get; set; }
        public int? GingivalMarginBuccalCenter { get; set; }
        public int? GingivalMarginBuccalRight { get; set; }
        public int? GingivalMarginLingualLeft { get; set; }
        public int? GingivalMarginLingualCenter { get; set; }
        public int? GingivalMarginLingualRight { get; set; }

        // Clinical Attachment Level (CAL) - calculated from PD and GM
        public int? ClinicalAttachmentLevelBuccalLeft { get; set; }
        public int? ClinicalAttachmentLevelBuccalCenter { get; set; }
        public int? ClinicalAttachmentLevelBuccalRight { get; set; }
        public int? ClinicalAttachmentLevelLingualLeft { get; set; }
        public int? ClinicalAttachmentLevelLingualCenter { get; set; }
        public int? ClinicalAttachmentLevelLingualRight { get; set; }

        // Mucogingival Junction (MGJ) - 6 points per tooth
        public int? MucogingivalJunctionBuccalLeft { get; set; }
        public int? MucogingivalJunctionBuccalCenter { get; set; }
        public int? MucogingivalJunctionBuccalRight { get; set; }
        public int? MucogingivalJunctionLingualLeft { get; set; }
        public int? MucogingivalJunctionLingualCenter { get; set; }
        public int? MucogingivalJunctionLingualRight { get; set; }

        // Additional measurements and conditions
        public bool IsBleedingBuccalLeft { get; set; }
        public bool IsBleedingBuccalCenter { get; set; }
        public bool IsBleedingBuccalRight { get; set; }
        public bool IsBleedingLingualLeft { get; set; }
        public bool IsBleedingLingualCenter { get; set; }
        public bool IsBleedingLingualRight { get; set; }

        public bool IsSuppurationBuccalLeft { get; set; }
        public bool IsSuppurationBuccalCenter { get; set; }
        public bool IsSuppurationBuccalRight { get; set; }
        public bool IsSuppurationLingualLeft { get; set; }
        public bool IsSuppurationLingualCenter { get; set; }
        public bool IsSuppurationLingualRight { get; set; }

        public int? MobilityGrade { get; set; } // 1, 2, 3
        public int? FurcationGrade { get; set; }

        public bool IsMissingTooth { get; set; }
        public bool HasImplant { get; set; }
        public string Notes { get; set; }

    }
}
