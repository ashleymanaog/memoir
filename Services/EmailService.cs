namespace ThomasianMemoir.Services
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.Extensions.Configuration;
    using MimeKit;
    using System.Threading.Tasks;

    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationCodeAsync(string toEmail, string username, string verificationCode)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Thomasian Memoir", smtpSettings["SmtpUsername"]));
            message.To.Add(new MailboxAddress("User", toEmail));
            message.Subject = "Password Reset Verification Code";

            // Load HTML template
            var templatePath = Path.Combine(Environment.CurrentDirectory, "App_Data", "verificationCodeEmailTemplate.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            htmlBody = htmlBody.Replace("{NAME}", username);
            htmlBody = htmlBody.Replace("{CODE}", verificationCode);

            var body = new TextPart("html")
            {
                Text = htmlBody
            };

            message.Body = body;

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSettings["SmtpUsername"], smtpSettings["SmtpPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
