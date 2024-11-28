using dentistAi_api.Models;

namespace dentistAi_api.Services
{
    public interface ITranscriptLogsService
    {
        Task AddTranscriptLogAsync(TranscriptLogs transcriptLog);
        Task<IEnumerable<TranscriptLogs>> GetAllTranscriptLogsAsync();
        Task<TranscriptLogs> GetTranscriptLogByIdAsync(string id);
    }
}
