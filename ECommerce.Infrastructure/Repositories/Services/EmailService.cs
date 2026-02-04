using ECommerce.Core.Services;
using ECommerce.Core.Sharing;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Tls;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Services
{
    public class EmailService : IEmailService
    {
        #region server support sending emails




        //private readonly EmailSettings _emailSettings;

        //public EmailService(IOptions<OldEmailSettings> emailSettings)
        //{
        //    _emailSettings = emailSettings.Value;
        //}

        //public async Task SendEmailAsync(string to, string subject, string body, IList<IFormFile>? attachments = null)
        //{
        //    MimeMessage? email = new MimeMessage
        //    {

        //        Subject = subject

        //    };

        //    email.To.Add(MailboxAddress.Parse(to));

        //    var builder = new BodyBuilder();

        //    if(attachments != null)
        //    {
        //        byte[] fileBytes;
        //        foreach (var file in attachments) 
        //        {
        //        if(file?.Length > 0) 
        //            { 
        //            using var ms = new MemoryStream();
        //                file.CopyTo(ms);    
        //                fileBytes = ms.ToArray();

        //                builder.Attachments.Add(fileName: file.FileName, fileBytes, MimeKit.ContentType.Parse(file.ContentType));
        //            }
        //        }
        //    }

        //    builder.HtmlBody =body;

        //    email.Body = builder.ToMessageBody();
        //    email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));

        //    using var stmp = new SmtpClient();
        //    stmp.Connect(_emailSettings.Host, port: _emailSettings.Port, SecureSocketOptions.StartTls);

        //    stmp.Authenticate(_emailSettings.Email, _emailSettings.Password);
        //    await stmp.SendAsync(email);

        //    stmp.Disconnect(true);


        //}

        #endregion 

        #region server does not support sending emails
        // if server does not support sending emails

        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, IList<IFormFile>? attachments = null)
        {
            
            var client = new SendGridClient(_emailSettings.Password); 

            var fromEmail = new EmailAddress(_emailSettings.Email, _emailSettings.DisplayName);
            var toEmail = new EmailAddress(to);

           
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, plainTextContent: string.Empty, htmlContent: body);

            
            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        var base64Content = Convert.ToBase64String(fileBytes);

                        msg.AddAttachment(file.FileName, base64Content);
                    }
                }
            }

           
            var response = await client.SendEmailAsync(msg);

            
            if (!response.IsSuccessStatusCode)
            {
                
                var error = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid Error: {error}");
            }
        }

        #endregion
    }
}
