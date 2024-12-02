using dentistAi_api.DTOs;
using dentistAi_api.Models;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }


        [HttpGet("GetPatient/{id}")]
        public async Task<IActionResult> GetPatientByIdAsync(string id, [FromQuery] string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("Tenant ID is required.");
            }

            var patient = await _patientService.GetPatientByIdAsync(id, tenantId);

            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found for tenant {tenantId}.");
            }

            return Ok(patient);
        }


        [HttpGet("GetPatientsByTenantId/{tenantId}")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientsByTenantId(string tenantId)
        {
            var patients = await _patientService.GetPatientsByTenantIdAsync(tenantId);
            return Ok(new { Success = true, Message = "Patients retrieved successfully.", Patients = patients });
        }

        [HttpPost("AddPatient")]
        public async Task<ActionResult> AddPatient([FromBody] Patient patient)
        {
            if (patient == null)
            {
                return BadRequest(new { Success = false, Message = "Invalid patient data." });
            }

            var result = await _patientService.AddPatientAsync(patient);

            if (result)
            {
                return Ok(new { Success = true, Message = "Patient added successfully." });
            }
            else
            {
                return StatusCode(500, new { Success = false, Message = "Failed to add patient." });
            }
        }


        [HttpPut("UpdatePatient/{id}")]
        public async Task<ActionResult> UpdatePatient(string id, [FromBody] Patient patient)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            patient.Id = objectId; // Ensure the ID is set correctly
            var success = await _patientService.UpdatePatientAsync(objectId.ToString(), patient);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Patient not found." });
            }
            return Ok(new { Success = true, Message = "Patient updated successfully." }); // Return success message
        }

        [HttpDelete("DeletePatient/{id}")]
        public async Task<ActionResult> DeletePatient(string id , string tenantId)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _patientService.DeletePatientAsync(objectId.ToString() , tenantId);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Patient not found." });
            }
            return Ok(new { Success = true, Message = "Patient soft deleted successfully." }); // Return success message
        }

        [HttpGet("GetPatientByPatientIdAsync/{patientId}")]
        public async Task<IActionResult> GetPatientByPatientIdAsync(int patientId, [FromQuery] string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("Tenant ID is required.");
            }

            var patient = await _patientService.GetPatientByPatientIdAsync(patientId, tenantId);

            if (patient == null)
            {
                return NotFound($"Patient with ID {patientId} not found for tenant {tenantId}.");
            }

            return Ok(patient);
        }

        [HttpGet("GetPatientsByDoctorIdsWithPagination")]
        public async Task<ActionResult<PaginatedResult<Patient>>> GetPatientsByDoctorIdsWithPaginationAsync(
         [FromQuery] IEnumerable<string> doctorIds,
         [FromQuery] string tenantId,
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10)
        {
            if (doctorIds == null || !doctorIds.Any() || string.IsNullOrEmpty(tenantId))
            {
                return BadRequest(new { Success = false, Message = "Doctor IDs and Tenant ID are required." });
            }

            var result = await _patientService.GetPatientsByDoctorIdsWithPaginationAsync(doctorIds, tenantId, pageNumber, pageSize);

            if (result == null || !result.Data.Any())
            {
                return NotFound(new { Success = false, Message = "No patients found for the provided doctor IDs." });
            }

            return Ok(new { Success = true, Patients = result.Data, TotalCount = result.TotalCount, PageNumber = result.PageNumber, PageSize = result.PageSize });
        }

    }
}