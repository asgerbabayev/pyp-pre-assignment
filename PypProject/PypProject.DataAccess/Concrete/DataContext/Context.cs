using Microsoft.EntityFrameworkCore;
using PypProject.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.DataAccess.Concrete.DataContext
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=PypProjectDb;Username=postgres;Password=12345;Port=5433");
        }

        public DbSet<ProductData> Datas { get; set; }
    }
}
