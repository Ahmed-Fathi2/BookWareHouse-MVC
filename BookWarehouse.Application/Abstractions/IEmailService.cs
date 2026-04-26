using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string username, string subjectEmail, string body);
    }
}
