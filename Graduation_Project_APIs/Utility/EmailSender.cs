using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace Graduation_Project_APIs.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bisproject39@gmail.com", "gqna ngqn qowy ncqi")
            };

            return client.SendMailAsync(
                new MailMessage(from: "bisproject39@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                );
        }
    }
}
