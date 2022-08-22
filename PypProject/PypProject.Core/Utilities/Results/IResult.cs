using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Utilities.Results
{
    public interface IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
