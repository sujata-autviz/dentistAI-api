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
        public async Task<ActionResult<Patient>> GetPatient(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var patient = await _patientService.GetPatientByIdAsync(objectId.ToString());
            if (patient == null)
            {
                return NotFound(new { Success = false, Message = "Patient not found." });
            }
            return Ok(new { Success = true, Message = "Patient retrieved successfully.", Patient = patient });
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
            await _patientService.AddPatientAsync(patient);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, new { Success = true, Message = "Patient added successfully.", Patient = patient });
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
        public async Task<ActionResult> DeletePatient(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Success = false, Message = "Invalid ID format." });
            }

            var success = await _patientService.DeletePatientAsync(objectId.ToString());
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Patient not found." });
            }
            return Ok(new { Success = true, Message = "Patient soft deleted successfully." }); // Return success message
        }
    }
}