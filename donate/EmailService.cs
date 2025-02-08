using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var emailSettings = _config.GetSection("EmailSettings");

        // Get values from configuration
        var senderEmail = emailSettings["SenderEmail"];
        var senderPassword = emailSettings["SenderPassword"];
        var smtpServer = emailSettings["SmtpServer"];
        var port = emailSettings["Port"];

        // Validate required fields
        if (string.IsNullOrEmpty(senderEmail))
            throw new ArgumentNullException(nameof(senderEmail), "Sender email cannot be null or empty.");

        if (string.IsNullOrEmpty(senderPassword))
            throw new ArgumentNullException(nameof(senderPassword), "Sender password cannot be null or empty.");

        if (string.IsNullOrEmpty(toEmail))
            throw new ArgumentNullException(nameof(toEmail), "Recipient email cannot be null or empty.");

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Donate", senderEmail));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = message
        };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(smtpServer, int.Parse(port), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail, senderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            // Log the exception or rethrow it as needed
            throw new Exception($"Failed to send email: {ex.Message}");
        }
    }
}
