using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using PypProject.Business.Abstract;
using PypProject.Business.Constants;
using PypProject.Core.Extensions;
using PypProject.Core.Utilities.Business;
using PypProject.Core.Utilities.File;
using PypProject.Core.Utilities.Mail;
using PypProject.Core.Utilities.Results;
using PypProject.Entities.Concrete;
using PypProject.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PypProject.Tests
{
    public class DataServiceFake : IDataService
    {
        private readonly List<ProductData> _datas;
        public DataServiceFake()
        {
            var path = Path.Combine("../../../data.json");
            string result;
            using (StreamReader sr = new StreamReader(path))
            {
                result = sr.ReadToEnd();
            }
            _datas = JsonConvert.DeserializeObject<List<ProductData>>(result);
        }

        public IResult Upload(IFormFile file)
        {
            var result = BusinessRules.Run(CheckTemplate(file), CheckFile(file));
            if (!result.Success) return new ErrorResult(result.Message);
            DataTable dt = FileHelper.ReadExcelFile(file);
            List<ProductData> datas = FileHelper.DataTableToList(dt);
            var json = JsonConvert.SerializeObject(datas);
            _datas.AddRange(datas);
            using (var sw = new StreamWriter("../../../data.json"))
            {
                sw.WriteLine(json);
            }
            return new Result(true, Messages.FileUploaded);
        }

        public IDataResult<List<Report>> SendReport(ReportDto getReportDto)
        {
            var result = BusinessRules.Run(CheckParamsValue(getReportDto),
                CheckEmails(getReportDto.AcceptorEmails),
                CheckDates(getReportDto.StartDate, getReportDto.EndDate), CheckType(getReportDto.Type));
            if (!result.Success) return new ErrorDataResult<List<Report>>(result.Message);
            if (getReportDto.Type != 4)
            {
                List<Report> reportSegment = GetSalesReport(getReportDto);
                return new SuccessDataResult<List<Report>>(reportSegment, Messages.ReportSended);
            }
            List<Report> reportDiscountsProduct = DiscountsProduct(getReportDto.StartDate, getReportDto.EndDate);
            return new SuccessDataResult<List<Report>>(reportDiscountsProduct, Messages.ReportSended);
        }

        #region Checking
        private IResult CheckReportsIsNull(List<Report> reports)
        {
            if (reports.Count == 0) return new ErrorResult(Messages.DatabaseIsEmpty);
            return new SuccessResult();
        }
        private IResult CheckParamsValue(ReportDto getReportDto)
        {
            if (getReportDto.Type == null) return new ErrorResult("Type is required");
            if (getReportDto.StartDate.ToString() == null) return new ErrorResult("Start date is required");
            if (getReportDto.EndDate.ToString() == null) return new ErrorResult("End date is required");
            if (getReportDto.AcceptorEmails == null) return new ErrorResult("Acceptor emails is required");
            return new SuccessResult();
        }
        private IResult CheckEmails(string[] emails)
        {
            Regex regex = new Regex("^([a-zA-Z]+[a-zA-z.!#$%&'*+-=?^`{|}~]{2})+[@]+code+[.]+edu+[.]+az+$");
            foreach (var email in emails)
                if (!regex.IsMatch(email)) return new ErrorResult(email + ": " + Messages.InvalidEmail);
            return new SuccessResult();
        }
        private IResult CheckDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) return new ErrorResult(Messages.StartDateCannotBeGreaterEndDate);
            return new SuccessResult();
        }
        private IResult CheckFile(IFormFile file)
        {
            // If the file is empty, error result is returned.
            if (file == null) return new ErrorResult(Messages.FileIsEmpty);

            // If the file extension is not xlxs or xls, error result is returned.
            if (file.ContentType != FileExtension.xlxs && file.ContentType != FileExtension.xls)
                return new ErrorResult(Messages.FileDoesNotMatchFormat);

            //If the file exceeds 5mb, error result is returned.
            if (file.Length / 1024 >= 5000) return new ErrorResult(Messages.FileCannotExceed5mb);
            return new SuccessResult();
        }
        private IResult CheckTemplate(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var pkg = new ExcelPackage(stream))
                {
                    string[] data = { "segment", "country", "product", "discount band", "units sold",
                        "manufacturing price","sale price","gross sales", "discounts","sales","cogs","profit","date"};

                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        int i, n = worksheet.Columns().Count();
                        for (i = 1; i < n; i++)
                        {
                            var result = CheckHeader(worksheet.Cell(1, i).Value.ToString(), data[i - 1]);

                            if (!result.Success)
                                return result;
                        }
                        n = worksheet.Rows().Count();
                        if (n == 3) return new ErrorResult(Messages.FileIsEmpty);
                    }
                }
                return new SuccessResult();
            }
        }
        private IResult CheckHeader(string header, string data)
        {
            if (header.ToString().Trim().ToLower().Equals(data.ToString().Trim().ToLower())) return new SuccessResult();
            return new ErrorResult(header + $" - {Messages.ColumnDoesNotMatchTemplate}");
        }

        private IResult CheckType(int? type)
        {
            if (!Enum.IsDefined(typeof(Entities.Enums.Type), type)) return new ErrorResult();
            return new SuccessResult();
        }

        #endregion

        public List<Report> GetSalesReport(ReportDto reportDto)
        {
            var result = _datas
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


        #region Discount Product
        public List<Report> DiscountsProduct(DateTime startDate, DateTime endDate)
        {
            var result = (from Datas in _datas
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
                              (from Datas0 in _datas
                               select new
                               {
                                   Datas0
                               }).Count())
                          }).ToList();
            return result;
        }

        #endregion

    }
}
