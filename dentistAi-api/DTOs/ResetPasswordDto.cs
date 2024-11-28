using MongoDB.Bson;

namespace dentistAi_api.DTOs
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } // Add a property for the token
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
