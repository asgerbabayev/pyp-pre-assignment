using Microsoft.AspNetCore.Http;
using PypProject.Core.DataAccess;
using PypProject.Entities.Concrete;
using PypProject.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.DataAccess.Abstract
{
    public interface IDataDal : IEntityRepository<ProductData>
    {
        void Upload(List<ProductData> datas);
        List<Report> DiscountsProduct(DateTime startDate, DateTime endDate);
        List<Report> GetSalesReport(ReportDto reportRequestDto);
    }
}
