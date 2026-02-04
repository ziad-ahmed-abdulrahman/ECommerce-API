using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body,IList<IFormFile>? attachments = null);
    }
}
