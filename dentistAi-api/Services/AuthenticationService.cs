using dentistAi_api.Data;
using dentistAi_api.DTOs;
using dentistAi_api.Helpers;
using dentistAi_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace dentistAi_api.Services
{
    public class AuthenticationService
    {
      
            private readonly MongoDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailHelper _emailHelper;
        private readonly IMongoCollection<Role> _roles;
        public AuthenticationService(MongoDbContext context , IConfiguration config , 
            IWebHostEnvironment env )
            {
                _context = context;
            _config = config;
            _roles = context.Roles;
            _emailHelper = new EmailHelper(config, env);
        }

            // Register a new user
            public async Task<bool> RegisterUserAsync(User user)
            {
                if (await _context.Users.Find(u => u.Email == user.Email).FirstOrDefaultAsync() != null)
                {
                    return false; // User already exists
                }
                user.IsActive = false;
                user.PasswordHash = HashPassword(user.PasswordHash);
                await _context.Users.InsertOneAsync(user);
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            var existingRole = await _roles.Find(r => r.Name == user.Role && r.TenantId == user.TenantId && !r.IsDeleted).FirstOrDefaultAsync();

            // If the role doesn't exist, create it
            if (existingRole == null)
            {
                var newRole = new Role
                {
                    Name = user.Role,
                    TenantId = user.TenantId,
                    IsDeleted = false
                };

                // Insert the new role into the Roles collection
                await _roles.InsertOneAsync(newRole);

                // Optionally, assign the new role to the user
                user.Role = newRole.Name;
            }
            //var emailSent = await _emailHelper.SendEmailOnUserCreation(fullName, user.Email, user.Id.ToString());

            return true;
            }

        // Login a user

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                                     .Find(u => u.Email == loginDto.Email)
                                     .FirstOrDefaultAsync();

            if (user == null || !VerifyPassword(user.PasswordHash, loginDto.Password) || !user.IsActive)
            {
                return null;
            }

            var userDto = new UserDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.LastName,
                LastName = user.LastName,
                Role = user.Role ,
                TenantId =user.TenantId
                // Add any additional fields to the UserDto as needed
            };

            var token = GenerateJwtToken(user);
            return new LoginResponseDto
            {
                Token = token,
                User = userDto
            };
        }

        //public async Task<string> LoginAsync(LoginDto loginDto)
        //{
        //    // Use FirstOrDefaultAsync to execute the query and get a single user
        //    var user = await _context.Users
        //                             .Find(u => u.Email == loginDto.Email)
        //                             .FirstOrDefaultAsync();

        //    if (user == null || !VerifyPassword(user.PasswordHash, loginDto.Password) || !user.IsActive)
        //    {
        //        return null;
        //    }

        //    return GenerateJwtToken(user);
        //}

        public async Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]);

                ClaimsPrincipal principal;
                try
                {
                    principal = tokenHandler.ValidateToken(resetPasswordDto.Token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                }
                catch (Exception)
                {
                    return (false, "Invalid token");
                }

                //var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                //if (userIdClaim == null)
                //{
                //    return (false, "Token does not contain user ID");
                //}

                //var userIdFromToken = userIdClaim.Value;

                //if (resetPasswordDto.UserId.ToString() != userIdFromToken)
                //{
                //    return (false, "User ID mismatch");
                //}

                // Retrieve the user from MongoDB
                var user = await _context.Users
                    .Find(u => u.Id == ObjectId.Parse(resetPasswordDto.UserId.ToString()) && !u.IsDeleted)
                    .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync to await the result

                if (user == null)
                {
                    return (false, "User not found");
                }

                // Verify the old password (if applicable)
                if (!VerifyPassword(user.PasswordHash, resetPasswordDto.OldPassword))
                {
                    return (false, "Incorrect old password");
                }

                // Update the password
                user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
                await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user); // Update user in MongoDB

                return (true, null);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return (false, "An unexpected error occurred");
            }
        }

        public async Task<(bool success, string message, object? data)> CreateNewPasswordAsync(CreateNewPasswordDto createNewPasswordDto)
        {
            try
            {
                // Find the user by username and ensure they are not deleted
                var user = await _context.Users
                    .Find(u => u.Email == createNewPasswordDto.Email && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return (false, "User not found", null);
                }

                // Hash the new password and update the user
                user.PasswordHash = HashPassword(createNewPasswordDto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                // Update the user in MongoDB
                await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

 
                    var loginToken = GenerateJwtToken(user);
                    await UpdateUserAfterOtpVerification(user);
                    return (true, "Password set successfully", new
                    {
                        token = loginToken,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        userName = user.UserName,
                        email = user.Email
                    });
                

                // Return without token for password reset
                return (true, "Password updated successfully", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateNewPasswordAsync: {ex.Message}");
                return (false, "An error occurred while setting the password", null);
            }
        }

        public async Task<bool> UpdateUserAfterOtpVerification(User user)
        {
            try
            {
                // Update user properties
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                // Update the user in MongoDB
                await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user after OTP verification: {ex.Message}");
                return false;
            }
        }


        // Hash the password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
        // Verify the password
        private bool VerifyPassword(string hashedPassword, string password)
        {
            return hashedPassword == HashPassword(password);
        }


        // Generate JWT Token
        private string GenerateJwtToken(User user)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
                // Add other claims as needed
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])); // Use a secure key
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "YourIssuer",
                    audience: "YourAudience",
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    
}