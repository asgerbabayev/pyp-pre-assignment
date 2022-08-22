using Microsoft.Extensions.DependencyInjection;
using PypProject.Core.Utilities.IoC;
using PypProject.Core.Utilities.Mail;
using PypProject.Core.Utilities.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Core.DependencyResolvers
{
    public class CoreModule : ICoreModule
    {
        public void Load(IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(MappingProfile));
            serviceCollection.AddSingleton<IMailHelper, MailHelper>();
        }
    }
}
