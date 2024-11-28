using dentistAi_api.Data;
using dentistAi_api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dentistAi_api.Services
{
    public class ToothService : IToothService
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Tooth> _teeth;

        public ToothService(MongoDbContext context)
        {
            _context = context;
            _teeth = _context.Teeth; // Assuming you have a Teeth collection in your MongoDbContext
        }

        public async Task<Tooth> GetToothByIdAsync(string id)
        {
            return await _teeth.Find(t => t.Id == ObjectId.Parse(id) && !t.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Tooth>> GetTeethByChartIdAsync(string chartId)
        {
            return await _teeth.Find(t => t.ChartId == chartId && !t.IsDeleted).ToListAsync();
        }

        public async Task<bool> AddToothAsync(Tooth tooth)
        {
            await _teeth.InsertOneAsync(tooth);
            return true; // Return true if the tooth was added successfully
        }

        public async Task<bool> UpdateToothAsync(string id, Tooth tooth)
        {
            var result = await _teeth.ReplaceOneAsync(t => t.Id == ObjectId.Parse(id) && !t.IsDeleted, tooth);
            return result.ModifiedCount > 0; // Return true if a document was modified
        }

        public async Task<bool> DeleteToothAsync(string id)
        {
            var tooth = await GetToothByIdAsync(id);
            if (tooth == null)
            {
                return false; // Tooth not found
            }

            tooth.IsDeleted = true; // Set the IsDeleted flag to true for soft delete
            await _teeth.ReplaceOneAsync(t => t.Id == ObjectId.Parse(id), tooth); // Update the tooth
            return true; // Return true if the tooth was soft deleted
        }
    }
}