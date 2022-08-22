using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Utilities.Mail
{
    public interface IMailHelper
    {
        void SendMail(MailRequest mailRequest);
    }
}
