using dentistAi_api.Data;
using dentistAi_api.DTOs;
using dentistAi_api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dentistAi_api.Services
{
    public class PeriodontalChartService : IPeriodontalChartService
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<PeriodontalChart> _charts;

        public PeriodontalChartService(MongoDbContext context)
        {
            _context = context;
            _charts = _context.PeriodontalCharts; // Assuming you have a PeriodontalCharts collection in your MongoDbContext
        }

        public async Task<PeriodontalChartDto> GetChartByIdAsync(string id, string tenantId)
        {
            // Fetch the chart from the database using the given id and tenantId
            var chart = await _charts
                .Find(c => c.Id == ObjectId.Parse(id) && !c.IsDeleted && c.TenantId == tenantId)
                .FirstOrDefaultAsync();

            // If the chart is found, map it to PeriodontalChartDto, otherwise return null
            if (chart == null)
            {
                return null;  // Or handle this case as needed, e.g., throw an exception or return an empty DTO
            }

            // Map the chart to a PeriodontalChartDto and return
            return new PeriodontalChartDto
            {
                Id = chart.Id.ToString(), // Convert ObjectId to string
                PatientID = chart.PatientID,
                IsDeleted = chart.IsDeleted,
                TenantId = chart.TenantId,
                ChartDate = chart.ChartDate,
                UpdatedAt = chart.UpdatedAt,
                DoctorId = chart.DoctorId,
                CreatedAt = chart.CreatedAt,
                Teeth = chart.Teeth
            };
        }


        public async Task<PeriodontalChart> GetChart(string id , string tenantId)
        {
          return await _charts
                 .Find(c => c.Id == ObjectId.Parse(id) && !c.IsDeleted && c.TenantId == tenantId)
                 .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<PeriodontalChartDto>> GetChartsByPatientAndTenantIdAsync(string patientId, string tenantId)
{
   

    var charts = await _charts.Find(c => c.PatientID == patientId && !c.IsDeleted && c.TenantId == tenantId).ToListAsync();
            return charts.Select(c => new PeriodontalChartDto
    {
        Id = c.Id.ToString(),
        PatientID = c.PatientID,
        TenantId = c.TenantId,
        DoctorId = c.DoctorId,
        Teeth = c.Teeth,
        // Check if UpdatedAt is present, otherwise fall back to CreatedAt
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt // Use UpdatedAt if available, else use CreatedAt
    }).OrderByDescending(x=>x.CreatedAt);
}

        public async Task<IEnumerable<PeriodontalChartDto>> GetChartsByPatientIdAsync(string patientId)
        {
            var charts = await _charts.Find(c => c.PatientID == patientId && !c.IsDeleted).ToListAsync();
            return charts.Select(c => new PeriodontalChartDto
            {
                Id = c.Id.ToString(), // Convert ObjectId to string
                PatientID = c.PatientID,
                IsDeleted = c.IsDeleted,
                TenantId = c.TenantId,
                ChartDate = c.ChartDate,
                UpdatedAt = c.UpdatedAt,
                DoctorId = c.DoctorId,
                CreatedAt  = c.CreatedAt,
                Teeth = c.Teeth,
              
            });
        }


        public async Task<string> AddChartAsync(PeriodontalChart chart)
        {
            // Insert the chart into the database
            await _charts.InsertOneAsync(chart);

            // Return the generated ChartId
            return chart.Id.ToString();
        }


        public async Task<bool> UpdateChartAsync(string id, PeriodontalChart chart)
        {
            var result = await _charts.ReplaceOneAsync(c => c.Id == ObjectId.Parse(id) && !c.IsDeleted, chart);
            return result.ModifiedCount > 0; // Return true if a document was modified
        }

        public async Task<bool> DeleteChartAsync(string id , string tenantId)
        {
            var chart = await GetChart(id, tenantId);
            if (chart == null)
            {
                return false; // Chart not found
            }

            chart.IsDeleted = true; // Set the IsDeleted flag to true for soft delete
            await _charts.ReplaceOneAsync(c => c.Id == ObjectId.Parse(id), chart); // Update the chart
            return true; // Return true if the chart was soft deleted
        }

        public async Task<(bool success , string? chartId)> AddOrUpdateTeethAsync(string? chartId, string tenantId, string patientId, string doctorId, List<Tooth> teeth)
        {
            PeriodontalChart chart;

            // Check if the chart exists
            if (!string.IsNullOrEmpty(chartId))
            {
                chart = await GetChart(chartId, tenantId);
                if (chart == null)
                {
                    return (false , null); // Chart not found
                }
            }
            else
            {
                // Create a new chart if chartId is not provided
                if (teeth == null || !teeth.Any())
                {
                    return (false, null); // No teeth provided to create a new chart
                }

                chart = new PeriodontalChart
                {
                     // Assuming all teeth have the same TenantId
                    PatientID = patientId,
                    TenantId = tenantId,// Use the provided PatientID as a string
                    ChartDate = DateTime.UtcNow,
                    DoctorId =  doctorId,// Set the current date as the chart date
                    Teeth = new List<Tooth>() // Initialize the teeth list
                };
            }

            // Update existing teeth or add new ones
            foreach (var tooth in teeth)
            {
                

                // Check if the tooth already exists in the database based on ToothNumber and ChartId
                var existingToothInDb = chart.Teeth
             .FirstOrDefault(t => t.ToothNumber == tooth.ToothNumber);


                if (existingToothInDb != null)
                {
                    // Update existing tooth in the database
                    existingToothInDb.MobilityGrade = tooth.MobilityGrade;
                    existingToothInDb.FurcationGrade = tooth.FurcationGrade;
                    existingToothInDb.IsMissingTooth = tooth.IsMissingTooth;
                    existingToothInDb.HasImplant = tooth.HasImplant;

                    // Update the additional properties related to the tooth
                    existingToothInDb.DistalBuccal = tooth.DistalBuccal;
                    existingToothInDb.Buccal = tooth.Buccal;
                    existingToothInDb.MesialBuccal = tooth.MesialBuccal;
                    existingToothInDb.DistalLingual = tooth.DistalLingual;
                    existingToothInDb.Lingual = tooth.Lingual;
                    existingToothInDb.MesialLingual = tooth.MesialLingual;

                    // Properties for teeth 6-11, 22-27 (likely for different sets of teeth)
                    existingToothInDb.DistalFacial = tooth.DistalFacial;
                    existingToothInDb.Facial = tooth.Facial;
                    existingToothInDb.MesialFacial = tooth.MesialFacial;
                    existingToothInDb.DistalPalatial = tooth.DistalPalatial;
                    existingToothInDb.Palatial = tooth.Palatial;
                    existingToothInDb.MesialPalatial = tooth.MesialPalatial;



                    // Update Clinical Attachment Level (CAL) properties
                    //existingToothInDb.ClinicalAttachmentLevelBuccalLeft =
                    //    (tooth.PocketDepthBuccalLeft ?? 0) + (tooth.GingivalMarginBuccalLeft ?? 0);
                    //existingToothInDb.ClinicalAttachmentLevelBuccalCenter =
                    //    (tooth.PocketDepthBuccalCenter ?? 0) + (tooth.GingivalMarginBuccalCenter ?? 0);
                    //existingToothInDb.ClinicalAttachmentLevelBuccalRight =
                    //    (tooth.PocketDepthBuccalRight ?? 0) + (tooth.GingivalMarginBuccalRight ?? 0);
                    //existingToothInDb.ClinicalAttachmentLevelLingualLeft =
                    //    (tooth.PocketDepthLingualLeft ?? 0) + (tooth.GingivalMarginLingualLeft ?? 0);
                    //existingToothInDb.ClinicalAttachmentLevelLingualCenter =
                    //    (tooth.PocketDepthLingualCenter ?? 0) + (tooth.GingivalMarginLingualCenter ?? 0);
                    //existingToothInDb.ClinicalAttachmentLevelLingualRight =
                    //    (tooth.PocketDepthLingualRight ?? 0) + (tooth.GingivalMarginLingualRight ?? 0);


                    existingToothInDb.ClinicalAttachmentLevelBuccalLeft = tooth.ClinicalAttachmentLevelBuccalLeft;
                    existingToothInDb.ClinicalAttachmentLevelBuccalCenter = tooth.ClinicalAttachmentLevelBuccalCenter;

                    existingToothInDb.ClinicalAttachmentLevelBuccalRight = tooth.ClinicalAttachmentLevelBuccalRight;

                    existingToothInDb.ClinicalAttachmentLevelLingualLeft = tooth.ClinicalAttachmentLevelLingualLeft;


                    existingToothInDb.ClinicalAttachmentLevelLingualCenter = tooth.ClinicalAttachmentLevelLingualCenter;


                    existingToothInDb.ClinicalAttachmentLevelLingualRight = tooth.ClinicalAttachmentLevelLingualRight;


                    // Update Mucogingival Junction (MGJ) measurements
                    existingToothInDb.MucogingivalJunctionBuccalLeft = tooth.MucogingivalJunctionBuccalLeft;
                    existingToothInDb.MucogingivalJunctionBuccalCenter = tooth.MucogingivalJunctionBuccalCenter;
                    existingToothInDb.MucogingivalJunctionBuccalRight = tooth.MucogingivalJunctionBuccalRight;
                    existingToothInDb.MucogingivalJunctionLingualLeft = tooth.MucogingivalJunctionLingualLeft;
                    existingToothInDb.MucogingivalJunctionLingualCenter = tooth.MucogingivalJunctionLingualCenter;
                    existingToothInDb.MucogingivalJunctionLingualRight = tooth.MucogingivalJunctionLingualRight;

                    // Update Bleeding properties
                    existingToothInDb.IsBleedingBuccalLeft = tooth.IsBleedingBuccalLeft;
                    existingToothInDb.IsBleedingBuccalCenter = tooth.IsBleedingBuccalCenter;
                    existingToothInDb.IsBleedingBuccalRight = tooth.IsBleedingBuccalRight;
                    existingToothInDb.IsBleedingLingualLeft = tooth.IsBleedingLingualLeft;
                    existingToothInDb.IsBleedingLingualCenter = tooth.IsBleedingLingualCenter;
                    existingToothInDb.IsBleedingLingualRight = tooth.IsBleedingLingualRight;

                    // Update Suppuration properties
                    existingToothInDb.IsSuppurationBuccalLeft = tooth.IsSuppurationBuccalLeft;
                    existingToothInDb.IsSuppurationBuccalCenter = tooth.IsSuppurationBuccalCenter;
                    existingToothInDb.IsSuppurationBuccalRight = tooth.IsSuppurationBuccalRight;
                    existingToothInDb.IsSuppurationLingualLeft = tooth.IsSuppurationLingualLeft;
                    existingToothInDb.IsSuppurationLingualCenter = tooth.IsSuppurationLingualCenter;
                    existingToothInDb.IsSuppurationLingualRight = tooth.IsSuppurationLingualRight;

                    // Update Pocket Depth measurements
           

                    // Update Gingival Margin measurements
                    existingToothInDb.GingivalMarginBuccalLeft = tooth.GingivalMarginBuccalLeft;
                    existingToothInDb.GingivalMarginBuccalCenter = tooth.GingivalMarginBuccalCenter;
                    existingToothInDb.GingivalMarginBuccalRight = tooth.GingivalMarginBuccalRight;
                    existingToothInDb.GingivalMarginLingualLeft = tooth.GingivalMarginLingualLeft;
                    existingToothInDb.GingivalMarginLingualCenter = tooth.GingivalMarginLingualCenter;
                    existingToothInDb.GingivalMarginLingualRight = tooth.GingivalMarginLingualRight;

         
                    var existingToothInChart = chart.Teeth.FirstOrDefault(t => t.ToothNumber == tooth.ToothNumber);
                    if (existingToothInChart != null)
                    {
                        chart.Teeth.Remove(existingToothInChart); // Remove the outdated tooth
                    }
                    chart.Teeth.Add(existingToothInDb);
                }

                else
                {

                    // Calculate Clinical Attachment Level (CAL) for the new tooth
                    //tooth.ClinicalAttachmentLevelBuccalLeft =
                    //    (tooth.PocketDepthBuccalLeft ?? 0) + (tooth.GingivalMarginBuccalLeft ?? 0);
                    //tooth.ClinicalAttachmentLevelBuccalCenter =
                    //    (tooth.PocketDepthBuccalCenter ?? 0) + (tooth.GingivalMarginBuccalCenter ?? 0);
                    //tooth.ClinicalAttachmentLevelBuccalRight =
                    //    (tooth.PocketDepthBuccalRight ?? 0) + (tooth.GingivalMarginBuccalRight ?? 0);
                    //tooth.ClinicalAttachmentLevelLingualLeft =
                    //    (tooth.PocketDepthLingualLeft ?? 0) + (tooth.GingivalMarginLingualLeft ?? 0);
                    //tooth.ClinicalAttachmentLevelLingualCenter =
                    //    (tooth.PocketDepthLingualCenter ?? 0) + (tooth.GingivalMarginLingualCenter ?? 0);
                    //tooth.ClinicalAttachmentLevelLingualRight =
                    //    (tooth.PocketDepthLingualRight ?? 0) + (tooth.GingivalMarginLingualRight ?? 0);
                    // Add new tooth to the chart
                    chart.Teeth.Add(tooth);

            
                }
            }

            // If a new chart was created, insert it into the database
            if (string.IsNullOrEmpty(chartId))
            {

                chartId = await AddChartAsync(chart); // Add the new chart
            }
            else
            {
                // Update the existing chart with the modified teeth list
                await UpdateChartAsync(chartId, chart);
            }

            return (true, chartId); // Return true if teeth were added or updated successfully
        }
    }
}