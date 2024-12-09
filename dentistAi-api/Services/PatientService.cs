using dentistAi_api.Data;
using dentistAi_api.DTOs;
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

        public async Task<Patient> GetPatientByIdAsync(string id, string tenantId)
        {
            return await _patients
                .Find(p => p.Id == ObjectId.Parse(id) && p.TenantId == tenantId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient> GetPatientByPatientIdAsync(int? patientId, string tenantId)
        {
            return await _patients
                .Find(p => p.PatientId == patientId && p.TenantId == tenantId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

         public async Task<int> GetNextPatientIdAsync(string tenantId , string doctorId)
    {
        var lastPatient = await _context.Patients
            .Find(p => p.TenantId == tenantId && p.DoctorId == doctorId)
            .SortByDescending(p => p.PatientId)
            .Limit(1)
            .FirstOrDefaultAsync();

        return (lastPatient?.PatientId ?? 0) + 1; // Increment the last PatientId
    }
        public async Task<PaginatedResult<PatientDto>> GetPatientsByDoctorIdsWithPaginationAsync(
          IEnumerable<string> doctorIds,
          string tenantId,
          int pageNumber,
          int pageSize,
          string searchTerm = null) // Optional search term
        {
            var filter = Builders<Patient>.Filter.And(
                Builders<Patient>.Filter.In(p => p.DoctorId, doctorIds),
                Builders<Patient>.Filter.Eq(p => p.TenantId, tenantId),
                Builders<Patient>.Filter.Eq(p => p.IsDeleted, false)
            );

            // If a search term is provided, add it to the filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Create the search filters for each field
                var searchFilter = Builders<Patient>.Filter.Or(
                    Builders<Patient>.Filter.Regex(p => p.Name, new BsonRegularExpression(searchTerm, "i")), // Case-insensitive search for Name
                    Builders<Patient>.Filter.Regex(p => p.Phone, new BsonRegularExpression(searchTerm, "i")), // Case-insensitive search for Phone
                    Builders<Patient>.Filter.Regex(p => p.PatientId, new BsonRegularExpression(searchTerm, "i")) // Case-insensitive search for Patient ID
                );
                if (int.TryParse(searchTerm, out int patientId))
                {
                    var patientIdFilter = Builders<Patient>.Filter.Eq(p => p.PatientId, patientId);
                    searchFilter = Builders<Patient>.Filter.Or(searchFilter, patientIdFilter); // Combine with existing search filters
                }
                // Handle DateOfBirth search separately (only if searchTerm is a valid date)
                if (DateTime.TryParse(searchTerm, out DateTime parsedDate))
                {
                    var dateOfBirthFilter = Builders<Patient>.Filter.Eq(p => p.DateOfBirth, parsedDate);
                    searchFilter = Builders<Patient>.Filter.Or(searchFilter, dateOfBirthFilter); // Combine with existing search filters
                }

                // Combine the search filter with the existing filter
                filter = Builders<Patient>.Filter.And(filter, searchFilter);
            }
            var totalCount = await _patients.CountDocumentsAsync(filter);
            var patients = await _patients
                .Find(filter)
                //.Sort(Builders<Patient>.Sort.Descending(p => p.CreatedAt))
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .Project(p => new PatientDto
                {
                    Id = p.Id.ToString(),  // Convert ObjectId to string
                    TenantId = p.TenantId,
                    PatientId = p.PatientId,
                    DoctorId = p.DoctorId,
                    Name = p.Name,
                    Age = p.Age,
                    Phone = p.Phone,
                    DateOfBirth = p.DateOfBirth,
                    IsDeleted = p.IsDeleted,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).Sort(Builders<Patient>.Sort.Descending(p => p.CreatedAt))
                .ToListAsync();

            return new PaginatedResult<PatientDto>
            {
                Data = patients,
                TotalCount = (int)totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<IEnumerable<Patient>> GetPatientsByTenantIdAsync(string tenantId)
        {
            return await _patients.Find(p => p.TenantId == tenantId && !p.IsDeleted).ToListAsync();
        }

        public async Task<PatientServiceResponse> AddPatientAsync(PatientDto patientDto)
        {
            var existingPatient = await _patients.Find(p =>
                p.PatientId == patientDto.PatientId &&
                p.TenantId == patientDto.TenantId &&
                p.DoctorId == patientDto.DoctorId &&
                !p.IsDeleted).FirstOrDefaultAsync();

            if (existingPatient != null)
            {
                return new PatientServiceResponse
                {
                    Success = false,
                    Message = "Duplicate PatientId detected. Patient was not added."
                };
            }

            var patient = new Patient
            {
                TenantId = patientDto.TenantId,
                PatientId = patientDto.PatientId,
                DoctorId = patientDto.DoctorId,
                Name = patientDto.Name,
                Age = patientDto.Age,
                Phone = patientDto.Phone,
                DateOfBirth = patientDto.DateOfBirth,
                IsDeleted = patientDto.IsDeleted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = patientDto.UpdatedAt
            };

            await _patients.InsertOneAsync(patient);
            return new PatientServiceResponse
            {
                Success = true,
                Message = "Patient added successfully."
            };
        }
        public async Task<bool> UpdatePatientAsync(string id, PatientDto patientDto)
        {
            var objectId = ObjectId.Parse(id);

            // Check if the patient exists
            var existingPatient = await _patients.Find(p => p.Id == objectId && !p.IsDeleted).FirstOrDefaultAsync();
            if (existingPatient == null)
            {
                return false; // Patient not found
            }

            // Update patient details
            var patient = new Patient
            {
                TenantId = patientDto.TenantId,
                PatientId = patientDto.PatientId,
                DoctorId = patientDto.DoctorId,
                Name = patientDto.Name,
                Age = patientDto.Age,
                Phone = patientDto.Phone,
                DateOfBirth = patientDto.DateOfBirth,
                IsDeleted = patientDto.IsDeleted,
                CreatedAt = patientDto.CreatedAt,
                UpdatedAt = DateTime.UtcNow, // You might want to set this to now
                Id = objectId
            };
            // Check if the updated PatientId is already in use by another patient
            var duplicatePatient = await _patients.Find(p =>
                p.PatientId == patientDto.PatientId &&
                p.TenantId == patientDto.TenantId &&
                p.DoctorId == patientDto.DoctorId &&
                p.Id != objectId &&
                !p.IsDeleted).FirstOrDefaultAsync();

            if (duplicatePatient != null)
            {
                return false; // Duplicate PatientId
            }

            var result = await _patients.ReplaceOneAsync(p => p.Id == objectId && !p.IsDeleted, patient);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeletePatientAsync(string id , string tenantId)
        {
            var patient = await GetPatientByIdAsync(id , tenantId);
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