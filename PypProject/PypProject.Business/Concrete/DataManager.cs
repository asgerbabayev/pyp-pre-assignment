using System;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using PypProject.Business.Abstract;
using PypProject.Business.Constants;
using PypProject.Core.Extensions;
using PypProject.Core.Utilities.Mail;
using PypProject.Core.Utilities.Results;
using PypProject.DataAccess.Abstract;
using PypProject.Entities.Concrete;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PypProject.Entities.DTOs;
using System.Linq;
using PypProject.Core.Utilities.Business;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using PypProject.Core.Utilities.File;

namespace PypProject.Business.Concrete
{
    public class DataManager : IDataService
    {
        private readonly IDataDal _dataDal;
        private readonly IMailHelper _mailHelper;
        private readonly ILoggerService _loggerService;

        public DataManager(IDataDal dataDal, IMailHelper mailHelper, ILoggerService loggerService)
        {
            _dataDal = dataDal;
            _mailHelper = mailHelper;
            _loggerService = loggerService;
        }


        #region Upload File
        public IResult Upload(IFormFile file)
        {
            //If the file is not compatible with the template, error result is returned.
            var result = BusinessRules.Run(CheckTemplate(file), CheckFile(file));
            if (!result.Success) { _loggerService.LogError(result.Message); return new ErrorResult(result.Message); }
            DataTable dt = FileHelper.ReadExcelFile(file);
            List<ProductData> datas = FileHelper.DataTableToList(dt);
            _dataDal.Upload(datas);
            _loggerService.LogInfo(Messages.FileUploaded);
            return new Result(true, Messages.FileUploaded);
        }
        #endregion

        #region Send Report
        public IDataResult<List<Report>> SendReport(ReportDto getReportDto)
        {
            var result = BusinessRules.Run(CheckParamsValue(getReportDto),
                CheckEmails(getReportDto.AcceptorEmails),
                CheckDates(getReportDto.StartDate, getReportDto.EndDate), CheckType(getReportDto.Type));
            if (!result.Success)
            {
                _loggerService.LogWarn(result.Message);
                return new ErrorDataResult<List<Report>>(result.Message);
            }
            if (getReportDto.Type != 4)
            {
                List<Report> reportSegment = _dataDal.GetSalesReport(getReportDto);
                var s = SendMail(getReportDto.Type, reportSegment, "Report", getReportDto.AcceptorEmails);
                if (!s.Success)
                {
                    _loggerService.LogError(s.Message);
                    return new ErrorDataResult<List<Report>>(s.Message);
                }
                _loggerService.LogInfo(Messages.ReportSended);
                return new SuccessDataResult<List<Report>>(Messages.ReportSended);
            }
            List<Report> reportDiscountsProduct = _dataDal.DiscountsProduct(getReportDto.StartDate, getReportDto.EndDate);
            var dp = SendMail(getReportDto.Type,reportDiscountsProduct, "Discount Percentages by Product", getReportDto.AcceptorEmails);
            if (!dp.Success)
            {
                _loggerService.LogError(dp.Message);
                return new ErrorDataResult<List<Report>>(dp.Message);
            }
            _loggerService.LogInfo(Messages.ReportSended);
            return new SuccessDataResult<List<Report>>(Messages.ReportSended);
        }
        #endregion

        #region Send Mail
        private IResult SendMail(int? type, List<Report> reports, string subject, string[] emails)
        {
            var result = CheckReportsIsNull(reports);
            if (!result.Success)
            {
                _loggerService.LogError(result.Message);
                return new ErrorResult(Messages.DatabaseIsEmpty);
            }
            MailRequest mail = new MailRequest();
            mail.Subject = subject;
            SeedTableHeadData(type,reports, mail);
            SeedTableBodyData(reports, mail);
            foreach (var email in emails)
            {
                mail.To = email;
                try
                {
                    _loggerService.LogInfo(email + " " + ReportForLogs(reports));
                    _mailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                    return new ErrorResult(Messages.ReportCannotSend);
                }
            }
            return new SuccessResult();
        }
        #endregion

        #region Checking
        private IResult CheckReportsIsNull(List<Report> reports)
        {
            if (reports is null) return new ErrorResult(Messages.DatabaseIsEmpty);
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

        #region Html Table Design
        private void SeedTableBodyData(List<Report> reports, MailRequest mail)
        {
            foreach (Report report in reports)
                if (report.Percent is null) mail.Content += ReportForSalesContent(report);
                else mail.Content += ReportForDiscountsProduct(report);
            mail.Content += "</tbody></table>";
        }
        private void SeedTableHeadData(int? type, List<Report> reports, MailRequest mail)
        {
            var percentIsExist = reports.FirstOrDefault().Percent;
            if (percentIsExist is null)
                mail.Content = HtmlTableForSales((Entities.Enums.Type)type);
            else
                mail.Content = HtmlTableForDiscountsProduct();
        }
        #endregion

        #region Report For Sales Email Content
        private string ReportForSalesContent(Report report)
        {
            string styleTd = @"border: 1px solid #ddd;
                               padding: 8px;";
            return $@"
<tr>
    <td style='{styleTd}'>{report.Title.ToUpper()}</td>
    <td style='{styleTd}'>{report.ProductCount}</td>
    <td style='{styleTd}'>{report.TotalSalePrice} $</td>
    <td style='{styleTd}'>{report.TotalProfitPrice} $</td>
    <td style='{styleTd}'>{report.TotalDiscountPrice} $</td>
</tr>";
        }
        private string HtmlTableForSales(Enum title)
        {
            string styleTable = @"font-family: arial, sans-serif;
                                  border-collapse: collapse;
                                  width: 100%;";
            string styleTh = @"padding-top: 12px;
                               padding-bottom: 12px;
                               text-align: left;
                               background-color: #4285F4;
                               color: white;
                               border: 1px solid #ddd;
                               padding: 8px;";
            return @$"
<table style='{styleTable}'>
    <thead>
        <tr>
            <th style='{styleTh}'>{title.ToString()}</th>
            <th style='{styleTh}'>Product Count</th>
            <th style='{styleTh}'>Total Sale Price</th>
            <th style='{styleTh}'>Total Profit Price</th>
            <th style='{styleTh}'>Total Discount Price</th>
        </tr>
    </thead>
    <tbody>";
        }
        #endregion

        #region Report For Discount Product Content
        private string HtmlTableForDiscountsProduct()
        {
            string styleTable = @"font-family: arial, sans-serif;
                                  border-collapse: collapse;
                                  width: 100%;";
            string styleTh = @"padding-top: 12px;
                               padding-bottom: 12px;
                               text-align: left;
                               background-color: #4285F4;
                               color: white;
                               border: 1px solid #ddd;
                               padding: 8px;";
            return @$"
<table style='{styleTable}'>
    <thead>
        <tr>
            <th style='{styleTh}'>Product Name</th>
            <th style='{styleTh}'>Discount Percentage</th>
        </tr>
    </thead>
    <tbody>";
        }
        private string ReportForDiscountsProduct(Report product)
        {
            string styleTd = @"border: 1px solid #ddd;
                               padding: 8px;";
            return $@"
<tr>
    <td style='{styleTd}'>{product.Title}</td>
    <td style='{styleTd}'>{product.Percent}</td>
</tr>";
        }
        #endregion

        #region Report For Logs
        private string ReportForLogs(List<Report> reports)
        {
            string data = "Göndərilmiş hesabatlar";
            foreach (Report report in reports)
            {
                data += $"\n--- {report.Title}, {report.ProductCount}, {report.TotalSalePrice}," +
                    $" {report.TotalDiscountPrice}, {report.TotalProfitPrice} {report.Percent} ---\n";
            }
            return data;
        }

        #endregion

    }
}
