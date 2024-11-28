using dentistAi_api.Models;
using MongoDB.Driver;

namespace dentistAi_api.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("roles");
        public IMongoCollection<Tenant> Tenants => _database.GetCollection<Tenant>("tenants");
        public IMongoCollection<Patient> Patients => _database.GetCollection<Patient>("patients");
        public IMongoCollection<PeriodontalChart> PeriodontalCharts => _database.GetCollection<PeriodontalChart>("periodontal_charts");
        public IMongoCollection<Tooth> Teeth => _database.GetCollection<Tooth>("teeth");
        public IMongoCollection<TranscriptLogs> TranscriptLogs => _database.GetCollection<TranscriptLogs>("transcriptLogs");
    }
}
