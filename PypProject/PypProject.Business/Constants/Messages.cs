using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Business.Constants
{
    public class Messages
    {
        public static string FileUploaded = "Fayl uğurla yükləndi";
        public static string FileIsEmpty = "Fayl boşdur";
        public static string ColumnDoesNotMatchTemplate = "Bu sütun şablonla uyğun deyil";
        public static string FileDoesNotMatchTemplate = "Fayl şablonla uyğun deyil";
        public static string FileDoesNotMatchFormat = "Fayl excel formatına uyğun deyil";
        public static string FileCannotExceed5mb = "Fayl 5mb-dan çox ola bilməz";
        public static string DataListed = "Siyahıya alındı";
        public static string ReportSended = "Hesabat Göndərildi";
        public static string ReportCannotSend = "Hesabat Göndəriləmədi. Xəta baş verdi";
        public static string DontHaveSuchReport = "Belə hesabat növü mövcud deyil";
        public static string DatabaseIsEmpty = "Sistemdə məlumat yoxdur";
        public static string InvalidEmail = "Email formatı yanlışdır";
        public static string StartDateCannotBeGreaterEndDate = "Başlanğıc tarix son tarixdən böyük ola bilməz";
    }
}
