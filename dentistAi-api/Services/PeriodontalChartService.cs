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

        public async Task<PeriodontalChart> GetChartByIdAsync(string id)
        {
            return await _charts.Find(c => c.Id == ObjectId.Parse(id) && !c.IsDeleted).FirstOrDefaultAsync();
        }

        //public async Task<IEnumerable<PeriodontalChart>> GetChartsByPatientIdAsync(string patientId)
        //{
        //    return await _charts.Find(c => c.PatientID == patientId && !c.IsDeleted).ToListAsync();
        //}
   
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
                CreatedAt  = c.CreatedAt,
                Teeth = c.Teeth,
              
            });
        }

        public async Task<bool> AddChartAsync(PeriodontalChart chart)
        {
            await _charts.InsertOneAsync(chart);
            return true; // Return true if the chart was added successfully
        }

        public async Task<bool> UpdateChartAsync(string id, PeriodontalChart chart)
        {
            var result = await _charts.ReplaceOneAsync(c => c.Id == ObjectId.Parse(id) && !c.IsDeleted, chart);
            return result.ModifiedCount > 0; // Return true if a document was modified
        }

        public async Task<bool> DeleteChartAsync(string id)
        {
            var chart = await GetChartByIdAsync(id);
            if (chart == null)
            {
                return false; // Chart not found
            }

            chart.IsDeleted = true; // Set the IsDeleted flag to true for soft delete
            await _charts.ReplaceOneAsync(c => c.Id == ObjectId.Parse(id), chart); // Update the chart
            return true; // Return true if the chart was soft deleted
        }

        public async Task<bool> AddOrUpdateTeethAsync(string chartId, string patientId, List<Tooth> teeth)
        {
            PeriodontalChart chart;

            // Check if the chart exists
            if (!string.IsNullOrEmpty(chartId))
            {
                chart = await GetChartByIdAsync(chartId);
                if (chart == null)
                {
                    return false; // Chart not found
                }
            }
            else
            {
                // Create a new chart if chartId is not provided
                if (teeth == null || !teeth.Any())
                {
                    return false; // No teeth provided to create a new chart
                }

                chart = new PeriodontalChart
                {
                    TenantId = teeth.First().TenantId, // Assuming all teeth have the same TenantId
                    PatientID = patientId, // Use the provided PatientID as a string
                    ChartDate = DateTime.UtcNow, // Set the current date as the chart date
                    Teeth = new List<Tooth>() // Initialize the teeth list
                };
            }

            // Update existing teeth or add new ones
            foreach (var tooth in teeth)
            {
                // Set the ChartId for each tooth
                tooth.ChartId = chart.Id.ToString();

                // Check if the tooth already exists in the database based on ToothNumber and ChartId
                var existingToothInDb = await _context.Teeth
                    .Find(t => t.ToothNumber == tooth.ToothNumber && t.ChartId == tooth.ChartId)
                    .FirstOrDefaultAsync();

                if (existingToothInDb != null)
                {
                    // Update existing tooth in the database
                    existingToothInDb.MobilityGrade = tooth.MobilityGrade;
                    existingToothInDb.FurcationGrade = tooth.FurcationGrade;
                    existingToothInDb.IsMissingTooth = tooth.IsMissingTooth;
                    existingToothInDb.HasImplant = tooth.HasImplant;

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
                    existingToothInDb.PocketDepthBuccalLeft = tooth.PocketDepthBuccalLeft;
                    existingToothInDb.PocketDepthBuccalCenter = tooth.PocketDepthBuccalCenter;
                    existingToothInDb.PocketDepthBuccalRight = tooth.PocketDepthBuccalRight;
                    existingToothInDb.PocketDepthLingualLeft = tooth.PocketDepthLingualLeft;
                    existingToothInDb.PocketDepthLingualCenter = tooth.PocketDepthLingualCenter;
                    existingToothInDb.PocketDepthLingualRight = tooth.PocketDepthLingualRight;

                    // Update Gingival Margin measurements
                    existingToothInDb.GingivalMarginBuccalLeft = tooth.GingivalMarginBuccalLeft;
                    existingToothInDb.GingivalMarginBuccalCenter = tooth.GingivalMarginBuccalCenter;
                    existingToothInDb.GingivalMarginBuccalRight = tooth.GingivalMarginBuccalRight;
                    existingToothInDb.GingivalMarginLingualLeft = tooth.GingivalMarginLingualLeft;
                    existingToothInDb.GingivalMarginLingualCenter = tooth.GingivalMarginLingualCenter;
                    existingToothInDb.GingivalMarginLingualRight = tooth.GingivalMarginLingualRight;

                    // Save the updated tooth back to the database
                    await _context.Teeth.ReplaceOneAsync(t => t.Id == existingToothInDb.Id, existingToothInDb);
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

                    // Insert new tooth into the database
                    await _context.Teeth.InsertOneAsync(tooth);
                }
            }

            // If a new chart was created, insert it into the database
            if (string.IsNullOrEmpty(chartId))
            {
                await AddChartAsync(chart); // Add the new chart
            }
            else
            {
                // Update the existing chart with the modified teeth list
                await UpdateChartAsync(chartId, chart);
            }

            return true; // Return true if teeth were added or updated successfully
        }
    }
}