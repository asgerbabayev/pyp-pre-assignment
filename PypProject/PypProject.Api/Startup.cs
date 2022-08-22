using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PypProject.Core.DependencyResolvers;
using PypProject.Core.Extensions;
using PypProject.Core.Utilities.IoC;
using Microsoft.OpenApi.Models;
using PypProject.Core.Utilities.Mail;
using NLog;
using System.IO;
using PypProject.Api.Extensions;
using PypProject.DataAccess.Concrete.DataContext;

namespace PypProject.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDependencyResolvers(new ICoreModule[] {
                new CoreModule()
            });


            var emailConfig = Configuration.GetSection("EmailConfiguration")
                  .Get<MailConfiguration>();
            services.AddSingleton(emailConfig);

            services.ConfigureCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PypProject.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PypProject.Api v1"));
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            LogManager.LoadConfiguration(Path.Combine(env.WebRootPath, "nlog.config"));
        }
    }
}
