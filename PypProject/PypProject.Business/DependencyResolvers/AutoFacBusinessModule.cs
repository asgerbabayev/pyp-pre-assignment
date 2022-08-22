using Autofac;
using PypProject.Business.Abstract;
using PypProject.Business.Concrete;
using PypProject.Core.Utilities.Mail;
using PypProject.DataAccess.Abstract;
using PypProject.DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Business.DependencyResolvers
{
    public class AutoFacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataManager>().As<IDataService>().SingleInstance();
            builder.RegisterType<EfDataDal>().As<IDataDal>().SingleInstance();
            builder.RegisterType<LoggerManager>().As<ILoggerService>().SingleInstance();
        }
    }
}
