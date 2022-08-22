using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.Extensions
{
    public static class FileExtension
    {
        public static string xlxs = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static string xls = "application/vnd.ms-excel";
        public static bool CheckType(this IFormFile file, string type)
        {
            if (file.ContentType.Contains(type)) return true;
            return false;
        }

        public static bool CheckSize(this IFormFile file, int size)
        {
            if (file.Length / 1024 > size) return true;
            return false;
        }

        public static void FileNameChange(this IFormFile file)
        {
            if (file.FileName.Length > 64)
            {
                file.FileName.Substring(file.FileName.Length - 64, 64);
            }
        }
    }
}
