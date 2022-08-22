using Microsoft.AspNetCore.Http;
using PypProject.Core.Utilities.Results;
using PypProject.Entities.Concrete;
using PypProject.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Business.Abstract
{
    public interface IDataService
    {
        IResult Upload(IFormFile file);
       IDataResult<List<Report>> SendReport(ReportDto getReportDto);
    }
}
