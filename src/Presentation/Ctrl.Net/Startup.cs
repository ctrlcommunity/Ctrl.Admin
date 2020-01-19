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
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;

namespace Ctrl.Net
{

    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()              
                         .SetBasePath(env.ContentRootPath)
                         .AddJsonFile(Path.Combine("Configs", "appsettings.json"), optional: true,
                                      reloadOnChange: true) //Settings for the application	
                         .AddJsonFile(Path.Combine("Configs", $"appsettings.{env.EnvironmentName}.json"),
                                      optional: true, reloadOnChange: true);
            Configuration = builder.Build();
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
                o.ConnectionString =Configuration["App:ConnectionStrings:0:ConnectionString"];
                o.Name = Configuration["App:ConnectionStrings:0:Name"];
            });
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(OperationLogAttribute));
                options.Filters.Add(typeof(ExceptionFilterAttribute));
                options.Filters.Add(typeof(WebPermissionFilter));
            }).AddNewtonsoftJson(options=> {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            //基于文件系统的密钥存储库（持久性保持密钥）
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine("login-keys")));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticHttpContext();
            #region Nlog
            LogManager.Configuration.Variables["connectionString"] = Configuration["App:ConnectionStrings:0:ConnectionString"];
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
