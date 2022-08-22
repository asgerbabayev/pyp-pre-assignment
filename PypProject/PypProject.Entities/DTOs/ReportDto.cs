using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Entities.DTOs
{
    public class ReportDto
    {
        public int? Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string[] AcceptorEmails { get; set; }
    }
}
