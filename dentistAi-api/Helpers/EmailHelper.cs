using System.Net.Mail;
using System.Net;

namespace dentistAi_api.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _templatePath;


        public EmailHelper(IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _configuration = configuration;
            _templatePath = Path.Combine(env.ContentRootPath, "Templates");

        }

        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                using var smtp = new SmtpClient
                {
                    Host = _configuration["AWS:SES:SmtpHost"],
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        _configuration["AWS:SES:Credentials:AccessKey"],
                        _configuration["AWS:SES:Credentials:SecretKey"]
                    )
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_configuration["AWS:SES:SourceEmail"], "Aut Review"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(toEmail, toName));

                await smtp.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendRegistrationEmailAsync(string name, string email, int userId, string otp)
        {
            var confirmationLink = $"{_configuration["SiteUrls:url"]}account/confirm-otp?email={Uri.EscapeDataString(email)}&otp={otp}";
            var template = await LoadTemplateAsync("RegistrationConfirmation");
            var body = template
                .Replace("{FulltName}", name)
                .Replace("{Otp}", otp)
                .Replace("{link}", confirmationLink);

            return await SendEmailAsync(
                email,
                $"{name}",
                "Confirm Your Registration",
                body
            );
        }

        public async Task<bool> SendForgotPasswordEmail(string name, string email, int userId, string otp)
        {
            var template = await LoadTemplateAsync("ForgotPassword");
            var body = template
                .Replace("{FullName}", name)
                .Replace("{Otp}", otp)
                .Replace("{link}", $"{_configuration["SiteUrl"]}account/reset-password?userId={userId}&token={otp}");

            return await SendEmailAsync(
               email,
                $"{name}",
                "Forgot Password Request",
                body
            );
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var filePath = Path.Combine(_templatePath, $"{templateName}.html");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Email template {templateName} not found.");
            }
            return await File.ReadAllTextAsync(filePath);
        }

        public async Task<bool> SendEmailOnUserCreation(string name, string email, string userId)
        {
            var template = await LoadTemplateAsync("UserCredentials");

            var body = template
                .Replace("{FullName}", name)
                .Replace("{UserId}", userId.ToString())
                .Replace("{SetupLink}", $"{_configuration["SiteUrls:url"]}account/set-password?email={Uri.EscapeDataString(email)}");


            return await SendEmailAsync(
                email,
                name,
                "Welcome to Our Service",
                body
            );
        }

    }
}
