using dentistAi_api.Data;
using dentistAi_api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dentistAi_api.Services
{
    public class TranscriptLogsService : ITranscriptLogsService
    {
        private readonly MongoDbContext _context;


        public TranscriptLogsService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task AddTranscriptLogAsync(TranscriptLogs transcriptLog)
        {
            await _context.TranscriptLogs.InsertOneAsync(transcriptLog);
        }

        public async Task<IEnumerable<TranscriptLogs>> GetAllTranscriptLogsAsync()
        {
            return await _context.TranscriptLogs.Find(_ => true).ToListAsync();
        }

        public async Task<TranscriptLogs> GetTranscriptLogByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null; // or throw an exception if you prefer
            }

            return await _context.TranscriptLogs.Find(log => log.Id == objectId).FirstOrDefaultAsync();
        }
    }

}