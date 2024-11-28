using dentistAi_api.Data;
using dentistAi_api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dentistAi_api.Services
{
    public class PatientService : IPatientService
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Patient> _patients;

        public PatientService(MongoDbContext context)
        {
            _context = context;
            _patients = _context.Patients; // Assuming you have a Patients collection in your MongoDbContext
        }

        public async Task<Patient> GetPatientByIdAsync(string id)
        {
            return await _patients.Find(p => p.Id == ObjectId.Parse(id) && !p.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsByTenantIdAsync(string tenantId)
        {
            return await _patients.Find(p => p.TenantId == tenantId && !p.IsDeleted).ToListAsync();
        }

        public async Task<bool> AddPatientAsync(Patient patient)
        {
            await _patients.InsertOneAsync(patient);
            return true; // Return true if the patient was added successfully
        }

        public async Task<bool> UpdatePatientAsync(string id, Patient patient)
        {
            var result = await _patients.ReplaceOneAsync(p => p.Id == ObjectId.Parse(id) && !p.IsDeleted, patient);
            return result.ModifiedCount > 0; // Return true if a document was modified
        }

        public async Task<bool> DeletePatientAsync(string id)
        {
            var patient = await GetPatientByIdAsync(id);
            if (patient == null)
            {
                return false; // Patient not found
            }

            patient.IsDeleted = true; // Set the IsDeleted flag to true for soft delete
            await _patients.ReplaceOneAsync(p => p.Id == ObjectId.Parse(id), patient); // Update the patient
            return true; // Return true if the patient was soft deleted
        }
    }
}