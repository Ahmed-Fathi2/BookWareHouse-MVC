using BookWarehouse.Application.Abstractions;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace BookWarehouse.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail,string username ,string subjectEmail, string body)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ahmedmhmd0237@gmail.com", "Readify Store");
            var subject = subjectEmail;
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = "";
            var htmlContent = body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

        }
    }
}
