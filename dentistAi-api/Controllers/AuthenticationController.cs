using dentistAi_api.DTOs;
using dentistAi_api.Models;
using dentistAi_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace dentistAi_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(User user)
        {
            var result = await _authenticationService.RegisterUserAsync(user);
            if (!result)
            {
                return BadRequest(new { Success = false, Message = "Registration failed. User may already exist." });
            }

            return Ok(new { Success = true, Message = "User registered successfully." });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authenticationService.LoginAsync(loginDto);
            if (loginResponse == null)
            {
                return Unauthorized("Invalid credentials or email not confirmed");
            }

            return Ok(new
            {
                success = true,
                message = "Login successful",
                data = loginResponse
            });
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, errorMessage) = await _authenticationService.ResetPasswordAsync(resetPasswordDto);

            if (success)
            {
                return Ok(new { message = "Password reset successfully" });
            }
            else
            {
                return BadRequest(new { message = errorMessage });
            }
        }

        [HttpPost("CreateNewPassword")]
        public async Task<IActionResult> CreateNewPassword([FromBody] CreateNewPasswordDto createNewPasswordDto)
        {
            var (success, message, data) = await _authenticationService.CreateNewPasswordAsync(createNewPasswordDto);

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new
            {
                success = true,
                message,
                data // This will include token and user details when Type is true, null otherwise
            });
        }
    }
}
