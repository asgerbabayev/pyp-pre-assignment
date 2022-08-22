using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Utilities.Mail
{
    public class MailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public bool IsHtmlBody { get; set; }
        public string Content { get; set; }
    }
}
