using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using PypProject.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Utilities.File
{
    public class FileHelper
    {

        #region Read Excel File
        public static DataTable ReadExcelFile(IFormFile file)
        {
            DataTable dt = new DataTable("Data");

            // We convert our excel file to stream
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);

                // We are reading our excel file from stream
                using (var workbook = new XLWorkbook(ms))
                {
                    var worksheet = workbook.Worksheet(1); // sheet 1

                    // We find how many columns are used in the sheet and add the columns to the DataTable,
                    // in the first row we have column headers
                    int i, n = worksheet.Columns().Count();
                    for (i = 1; i <= n; i++)
                    {
                        dt.Columns.Add(worksheet.Cell(1, i).Value.ToString());
                    }

                    // We find how many rows are used on the page and add our rows to the DataTable
                    n = worksheet.Rows().Count();
                    for (i = 2; i <= n; i++)
                    {
                        DataRow dr = dt.NewRow();

                        int j, k = worksheet.Columns().Count();
                        for (j = 1; j < k; j++)
                        {
                            // i= row index, j=column index, for closedXML worksheet the indexes start from 1,
                            // but for the datatable it starts from 0, so we call it j-1
                            dr[j - 1] = worksheet.Cell(i, j).Value;
                        }

                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }
        #endregion
        #region We convert table to list
        public static List<ProductData> DataTableToList(DataTable dt)
        {
            List<ProductData> convertedList = new List<ProductData>();
            convertedList = (from DataRow dr in dt.Rows
                             select new ProductData()
                             {
                                 Segment = dr["Segment"].ToString().Trim().ToLower(),
                                 Country = dr["Country"].ToString().Trim().ToLower(),
                                 Product = dr["Product"].ToString().Trim().ToLower(),
                                 DiscountBand = dr["Discount Band"].ToString().Trim().ToLower(),
                                 UnitsSold = Convert.ToDecimal(dr["Units Sold"].ToString().Trim().ToLower()),
                                 ManufacturingPrice = Convert.ToDecimal(dr["Manufacturing Price"].ToString().Trim().ToLower()),
                                 SalePrice = Convert.ToDecimal(dr["Sale Price"].ToString().Trim().ToLower()),
                                 GrossSales = Convert.ToDecimal(dr["Gross Sales"].ToString().Trim().ToLower()),
                                 Discounts = Convert.ToDecimal(dr["Discounts"].ToString().Trim().ToLower()),
                                 Sales = Convert.ToDecimal(dr["Sales"].ToString().Trim().ToLower()),
                                 COGS = Convert.ToDecimal(dr["COGS"].ToString().Trim().ToLower()),
                                 Profit = Convert.ToDecimal(dr["Profit"].ToString().Trim().ToLower()),
                                 Date = Convert.ToDateTime(dr["Date"].ToString().Trim().ToLower())
                             }).ToList();
            return convertedList;
        }
        #endregion
    }
}
