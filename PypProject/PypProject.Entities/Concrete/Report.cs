using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Entities.Concrete
{
    public class Report
    {
        public string Title { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalSalePrice { get; set; }
        public decimal TotalProfitPrice { get; set; }
        public decimal? TotalDiscountPrice { get; set; }
        public int? Percent { get; set; }
    }
}
