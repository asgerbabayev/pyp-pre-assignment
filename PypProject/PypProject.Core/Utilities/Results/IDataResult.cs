using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Utilities.Results
{
    public interface IDataResult<E>
    {
        E Data { get; }
        bool Success { get; }
    }
}
