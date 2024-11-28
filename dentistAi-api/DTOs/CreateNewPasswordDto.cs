using dentistAi_api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace dentistAi_api.DTOs
{
    public class CreateNewPasswordDto
    {
        [Required]
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
