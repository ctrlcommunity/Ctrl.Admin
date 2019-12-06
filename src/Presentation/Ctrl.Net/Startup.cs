using Ctrl.Core.Core.DependencyInjection;
using Ctrl.Core.Core.Http;
using Ctrl.Core.PetaPoco.DependencyInjection;
using Ctrl.Core.Web.Attributes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System.Reflection;

namespace Ctrl.Net
{

    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessors();
            services.AddMemoryCache();

            //注册Cookie认证服务
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o =>
            {
                o.LoginPath = new PathString("/sysManage/Account/Login");
                o.AccessDeniedPath = new PathString("/sysManage/Home/NotFounds");
                o.SlidingExpiration = true;
            });
            services.BatchRegisterService(Assembly.Load("Ctrl.Domain.DataAccess"), "Repository", ServiceLifetime.Scoped);
            services.BatchRegisterService(Assembly.Load("Ctrl.Domain.Business"), "Logic", ServiceLifetime.Scoped);
            services.BatchRegisterService(Assembly.Load("Ctrl.Core.Business"), "Logic", ServiceLifetime.Scoped);

            services.AddPetaPoco(o=> {
                o.ConnectionString = "Data Source=.;Initial Catalog=Ctrl.Net;User ID=sa;Password=sa;MultipleActiveResultSets=true;";
                o.Name = "mssql";
            });
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(OperationLogAttribute));
                options.Filters.Add(typeof(ExceptionFilterAttribute));
                options.Filters.Add(typeof(WebPermissionFilter));
            }).AddNewtonsoftJson(options=> {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticHttpContext();
            #region Nlog
            LogManager.Configuration.Variables["connectionString"] = "Data Source=.;Initial Catalog=Ctrl.Net;User ID=sa;Password=sa;MultipleActiveResultSets=true;";
            #endregion
            app.UseCookiePolicy();
            var Provider = new FileExtensionContentTypeProvider();
            Provider.Mappings[".less"] = "text/css";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = Provider
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "sysManage", "sysManage",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
