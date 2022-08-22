using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Utilities.Mail
{
    public class MailHelper : IMailHelper
    {
        private readonly MailConfiguration _mailConfiguration;
        private readonly IHostEnvironment _env;

        public MailHelper(MailConfiguration mailConfiguration, IHostEnvironment env)
        {
            _mailConfiguration = mailConfiguration;
            _env = env;
        }


        public void SendMail(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailConfiguration.From);
            email.To.Add(MailboxAddress.Parse(mailRequest.To));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Content;
            email.Body = builder.ToMessageBody();
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailConfiguration.SmtpServer, _mailConfiguration.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailConfiguration.Username, _mailConfiguration.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
