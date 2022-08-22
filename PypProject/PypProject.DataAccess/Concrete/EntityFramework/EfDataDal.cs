using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using PypProject.Core.DataAccess.EntityFramework;
using PypProject.DataAccess.Abstract;
using PypProject.DataAccess.Concrete.DataContext;
using PypProject.Entities.Concrete;
using PypProject.Core.Utilities.File;
using PypProject.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace PypProject.DataAccess.Concrete.EntityFramework
{
    public class EfDataDal : EfEntityRepositoryBase<ProductData, Context>, IDataDal
    {
        #region Upload File
        public void Upload(List<ProductData> datas)
        {
            using (Context context = new Context())
            {
                foreach (var data in datas)
                {
                    context.Datas.Add(data);
                }
                context.SaveChanges();
            }
        }
        #endregion

        #region Get Sales Report
        public List<Report> GetSalesReport(ReportDto reportDto)
        {
            using (Context context = new())
            {
                var result = context.Datas
                    .Where(x => x.Date >= reportDto.StartDate && x.Date <= reportDto.EndDate)
                    .GroupBy(x => reportDto.Type == 1 ? x.Segment : reportDto.Type == 2 ? x.Country : reportDto.Type == 3 ? x.Product : null)
               .Select(x => new Report
               {
                   Title = x.Key,
                   ProductCount = x.Count(),
                   TotalSalePrice = x.Sum(s => s.Sales),
                   TotalProfitPrice = x.Sum(s => s.Profit),
                   TotalDiscountPrice = x.Sum(s => s.Discounts),
               })
               .ToList();

                return result;
            }

        }
        #endregion

        #region Discount Product
        public List<Report> DiscountsProduct(DateTime startDate, DateTime endDate)
        {
            using (Context context = new Context())
            {
                var result = (from Datas in context.Datas
                              where
                                Datas.Date >= startDate && Datas.Date <= endDate
                              group Datas by new
                              {
                                  Datas.Product
                              } into g
                              select new Report
                              {
                                  Title = g.Key.Product,
                                  Percent = (g.Count(p => p.Discounts != null) * 100 /
                                  (from Datas0 in context.Datas
                                   select new
                                   {
                                       Datas0
                                   }).Count())
                              }).ToList();
                return result;
            }
        }
        #endregion
    }
}
